#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System
open System.IO

open Fake.DotNet
open Fake.Core
open Fake.IO
open Fake.Tools
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

let clientPath = "./src/Client" |> Path.getFullName
let serverPath = "./src/Server/" |> Path.getFullName
let deployDir = "./deploy"

let dotnetcliVersion = DotNet.getSDKVersionFromGlobalJson()
let install = lazy DotNet.install (fun info -> { DotNet.Versions.FromGlobalJson info with Version = DotNet.Version dotnetcliVersion })
let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd
let inline dotnetSimple arg = DotNet.Options.lift install.Value arg

let platformTool tool winTool =
    let tool = if Environment.isUnix then tool else winTool
    match ProcessUtils.tryFindFileOnPath tool with
    | Some t -> t
    | _ ->
        let errorMsg =
            tool + " was not found in path. " +
            "Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
        failwith errorMsg

do if not Environment.isWindows then
    // We have to set the FrameworkPathOverride so that dotnet sdk invocations know
    // where to look for full-framework base class libraries
    let mono = platformTool "mono" "mono"
    let frameworkPath = IO.Path.GetDirectoryName(mono) </> ".." </> "lib" </> "mono" </> "4.5"
    Environment.setEnvironVar "FrameworkPathOverride" frameworkPath

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let runTool cmd args workingDir =
    Command.RawCommand(cmd, Arguments.OfArgs[args])
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.withTimeout TimeSpan.MaxValue
    |> Proc.run

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (withWorkDir workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let openBrowser url =
    Command.ShellCommand(url)
    |> CreateProcess.fromCommand
    |> CreateProcess.withTimeout TimeSpan.MaxValue
    |> Proc.run
    |> ignore

Target.create "Clean" (fun _ ->
    Shell.cleanDirs [deployDir]
)

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    runTool nodeTool "--version" __SOURCE_DIRECTORY__ |> ignore
    printfn "Yarn version:"
    runTool yarnTool "--version" __SOURCE_DIRECTORY__ |> ignore
    runTool yarnTool "install" __SOURCE_DIRECTORY__ |> ignore
)

Target.create "RestoreServer" (fun _ ->
    runDotNet "restore" serverPath
)

Target.create "Build" (fun _ ->
    runDotNet "build" serverPath
    runDotNet "restore" clientPath
    runDotNet "fable webpack-cli -- --config src/Client/webpack.config.js -p" clientPath
)

Target.create "Run" (fun _ ->
    runDotNet "restore" clientPath
    let server = async {
        runDotNet "watch run" serverPath
    }
    let client = async {
        runDotNet "fable webpack-cli -- --config src/Client/webpack.config.js -p" clientPath
    }
    let browser = async {
        Threading.Thread.Sleep 5000
        openBrowser "http://localhost:8085"
    }

    [ server; client; browser ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

// --------------------------------------------------------------------------------------
// Release Scripts
let dockerUser = Environment.environVar "DockerUser"
let dockerPassword = Environment.environVar "DockerPassword"
let dockerLoginServer = Environment.environVar "DockerLoginServer"
let dockerImageName = Environment.environVar "DockerImageName"

let releaseNotes = File.ReadAllLines "RELEASE_NOTES.md"

let releaseNotesData =
    releaseNotes
    |> ReleaseNotes.parseAll

let release = List.head releaseNotesData

Target.create "SetReleaseNotes" (fun _ ->
    let lines = [
            "module internal ReleaseNotes"
            ""
            (sprintf "let Version = %s" release.NugetVersion)
            ""
            (sprintf "let IsPrerelease = %b" (release.SemVer.PreRelease <> None))
            ""
            "let Notes = "] @ Array.toList releaseNotes @ [""]
    File.WriteAllLines("src/Client/ReleaseNotes.fs",lines)
)

Target.create "PrepareRelease" (fun _ ->
    Git.Branches.checkout "" false "master"
    Git.CommandHelper.directRunGitCommand "" "fetch origin" |> ignore
    Git.CommandHelper.directRunGitCommand "" "fetch origin --tags" |> ignore

    Git.Staging.stageAll ""
    Git.Commit.exec "" (sprintf "Bumping version to %O" release.NugetVersion)
    Git.Branches.pushBranch "" "origin" "master"

    let tagName = string release.NugetVersion
    Git.Branches.tag "" tagName
    Git.Branches.pushTag "" "origin" tagName

    let result =
        Process.execSimple (fun info ->
            { info with
                FileName = "docker"
                Arguments = sprintf "tag %s/%s %s/%s:%s" dockerUser dockerImageName dockerUser dockerImageName release.NugetVersion}) TimeSpan.MaxValue
    if result <> 0 then failwith "Docker tag failed"
)

Target.create "Bundle" (fun _ ->
    let serverDir = Path.combine deployDir "Server"
    let clientDir = Path.combine deployDir "Client"
    let publicDir = Path.combine clientDir "public"
    let dataDir = serverDir </> "Data"

    let dotnetOpts = install.Value (DotNet.Options.Create())
    let result =
        Process.execSimple (fun info ->
            { info with
                FileName = dotnetOpts.DotNetCliPath
                WorkingDirectory = serverPath
                Arguments = "publish -c Release -o \"" + Path.getFullName serverDir + "\"" }) TimeSpan.MaxValue
    if result <> 0 then failwith "Publish failed"

    Shell.copyDir publicDir "src/Client/public" FileFilter.allFiles
    !! "src/Server/Data/**/*.*" |> Shell.copyFiles dataDir
)

Target.create "CreateDockerImage" (fun _ ->
    if String.IsNullOrEmpty dockerUser then
        failwithf "docker username not given."
    if String.IsNullOrEmpty dockerImageName then
        failwithf "docker image Name not given."
    let result =
        Process.execSimple (fun info ->
            { info with
                FileName = "docker"
                UseShellExecute = false
                Arguments = sprintf "build -t %s/%s ." dockerUser dockerImageName }) TimeSpan.MaxValue
    if result <> 0 then failwith "Docker build failed"
)

Target.create "Deploy" (fun _ ->
    let result =
        Process.execSimple (fun info ->
            { info with
                FileName = "docker"
                WorkingDirectory = deployDir
                Arguments = sprintf "login %s --username \"%s\" --password \"%s\"" dockerLoginServer dockerUser dockerPassword }) TimeSpan.MaxValue
    if result <> 0 then failwith "Docker login failed"

    let result =
        Process.execSimple (fun info ->
            { info with
                FileName = "docker"
                WorkingDirectory = deployDir
                Arguments = sprintf "push %s/%s" dockerUser dockerImageName }) TimeSpan.MaxValue
    if result <> 0 then failwith "Docker push failed"
)

open Fake.Core.TargetOperators

"Clean"
    ==> "InstallClient"
    ==> "Build"
    ==> "Bundle"
    ==> "SetReleaseNotes"
    ==> "CreateDockerImage"
    ==> "PrepareRelease"
    ==> "Deploy"

"InstallClient"
    ==> "RestoreServer"
    ==> "Run"

Target.runOrDefaultWithArguments "Build"
