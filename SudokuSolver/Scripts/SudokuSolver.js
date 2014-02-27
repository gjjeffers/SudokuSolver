
$(document).ready(function () {
    $('#selectFile').change(function (event) {
    var selected_file = document.getElementById('selectFile').files[0];
    var e = new FileReader();
    e.onload = function () {
        var url = "/index/solve?start=" + e.result;
        $.post(url, function (data) {
            $("#puzzleGrid").empty();
            $("#puzzleGrid").hide().html(data).fadeIn('slow');
        });
    };
    e.onerror = function (evt) {
        alert("Error reading file!");
    }
    e.readAsText(selected_file);
    });

});
