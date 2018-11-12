#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System

open Fake
open Fake.Core
open Fake.DotNet
open Fake.IO

let serverPath = Path.getFullName "./src/Server"
let clientPath = Path.getFullName "./src/Client"
let deployDir = Path.getFullName "./deploy"

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

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let install = lazy DotNet.install DotNet.Versions.Release_2_1_300

let inline withWorkDir wd =
    DotNet.Options.lift install.Value
    >> DotNet.Options.withWorkingDirectory wd

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

let dockerUser = Environment.environVarOrDefault "DockerUser" ""
let dockerPassword = Environment.environVarOrDefault "DockerPassword" ""
let dockerLoginServer = Environment.environVarOrDefault "DockerLoginServer" ""
let dockerImageName =  Environment.environVarOrDefault "DockerImageName" ""

// --------------------------------------------------------------------------------------
// Release Scripts

Target.create "Bundle" (fun _ ->
    let serverDir = Path.combine deployDir "Server"
    let clientDir = Path.combine deployDir "Client"
    let publicDir = Path.combine clientDir "public"

    let publishArgs = sprintf "publish -c Release -o %s" serverDir
    runDotNet publishArgs serverPath

    Shell.copyDir publicDir "src/Client/public" FileFilter.allFiles
)

Target.create "CreateDockerImage" (fun _ ->
    if String.IsNullOrEmpty dockerUser then
        failwithf "docker username not given."
    if String.IsNullOrEmpty dockerImageName then
        failwithf "docker image Name not given."
    Command.RawCommand("docker", Arguments.OfArgs[sprintf "build -t %s/%s ." dockerUser dockerImageName])
    |> CreateProcess.fromCommand
    |> CreateProcess.withTimeout TimeSpan.MaxValue
    |> Proc.run
    |> ignore
)

Target.create "Deploy" (fun _ ->
    Command.RawCommand("docker", Arguments.OfArgs[sprintf "login %s --username %s --password %s ." dockerLoginServer dockerUser dockerPassword])
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory deployDir
    |> CreateProcess.withTimeout TimeSpan.MaxValue
    |> Proc.run
    |> ignore

    Command.RawCommand("docker", Arguments.OfArgs[sprintf "push %s/%s" dockerUser dockerImageName])
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory deployDir
    |> CreateProcess.withTimeout TimeSpan.MaxValue
    |> Proc.run
    |> ignore
)

open Fake.Core.TargetOperators

"Clean"
    ==> "InstallClient"
    ==> "Build"
    ==> "Bundle"
    ==> "CreateDockerImage"
    ==> "Deploy"

"InstallClient"
    ==> "RestoreServer"
    ==> "Run"

Target.runOrDefaultWithArguments "Build"
