 function yeastTableSearchAndSort(yeasts) {
    $(window).bind("load", function() {
        var options = {
            valueNames: [ 'company', 'yeastId', 'name', 'type', 'format' ]
        };
        
        var yeastList = new List('yeast-profile-table', options);
    });
 }