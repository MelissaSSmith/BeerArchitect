module.exports = {
    setUpYeastProfileTable: function (obj) {
        var options = {
            valueNames: [ 'company', 'yeastId', 'name', 'type', 'format' ]
        };
        
        var yeastList = new List('yeast-profile-table', options);
    }
};

$(window).bind("load", function() {
    var options = {
        valueNames: [ 'company', 'yeastId', 'name', 'type', 'format' ]
    };
    
    var yeastList = new List('yeast-profile-table', options);
 });