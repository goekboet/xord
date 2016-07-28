var xord = {
    service: "api/suggest/"
}

function query(e) {
    e.preventDefault();
    $.get(xord.service + xord.input.val(), function(suggestions) 
    {
        xord.output.empty();
        $.each(suggestions, function(i, suggestion) 
        {
            var listitem = $('<li>' + suggestion + '</li>');
            xord.output.append(listitem);
        })
    })
}

$(function() {
    xord.input = $('#xord-input input[name=query]');
    xord.output = $('#xord-output');
    var form = $('#xord-input')
    form.submit(query);
    $('#xord-input button[name=go]')
        .click(function () {
            console.log("hallå");
            form.submit()
        });
})