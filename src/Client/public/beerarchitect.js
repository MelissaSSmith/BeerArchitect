function yeastTableSearchAndSort() {
    $(window).bind("load", function() {
        var options = {
            valueNames: [ 'company', 'yeastId', 'name', 'type', 'format' ],
            page: 15,
            pagination: true
        };
        
        var yeastList = new List('yeast-profile-table', options);
    });
}

function fermentableTableSearchAndSort() {
    $(window).bind("load", function() {
        var options = {
            valueNames: [ 'name', 'country', 'category', 'type', 'degrees-lovibond', 'ppg' ],
            page: 15,
            pagination: true
        };
        
        var fermentableList = new List('fermentable-table', options);
    });
}

function hopTableSearchAndSort() {
    $(window).bind("load", function() {
    });
}