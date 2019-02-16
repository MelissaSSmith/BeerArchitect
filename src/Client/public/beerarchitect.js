 function yeastTableSearchAndSort(yeasts) {
    $(window).bind("load", function() {
        var options = {
            valueNames: [ 'company', 'yeastId', 'name', 'type', 'format' ],
            page: 15,
            pagination: true
        };
        
        var yeastList = new List('yeast-profile-table', options);
    });
 }