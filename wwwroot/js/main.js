$(document).on("click", ".modal-btn", function (e) {
    e.preventDefault();

    let url = $(this).attr("href");

    fetch(url)
        .then(response => {

            if (response.ok) {
                return response.text()
            }
            else {
                alert("Bilinmedik bir xeta bas verdi!")
            }
        })
        .then(data => {
            $("#quickModal .modal-dialog").html(data)
            $("#quickModal").modal('show');
        })
})
$(document).on("click", ".addtobasket", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url)
        .then(response => {
            if (!response.ok) {
                alert("Xeta Bash Verdi")
            }
            return response.text()
        })
        .then(data => {
            $(".cart-block").html(data)
        })
})


$(document).on("click", ".removefrombasket", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url)
        .then(response => {
            if (!response.ok) {
                alert("xeta bas verdi")
                return
            }
            return response.text()
        })
        .then(data => {
            $(".cart-block").html(data)
        })
})

$(document).on("keyup", "#inputSearch", function (e) {
    var searchedBook = $(this).val()

    $("#searchArea li").remove();
    $.ajax({
        url: "/home/search?search=" + searchedBook,
        type: 'GET',
        success: function (html) {
            
            $("#searchArea").append(html)
        }
    });

})


