$(document).on("click", ".modal-btn", function (e) {
    e.preventDefault();

    let url = $(this).attr("href");

    fetch(url)
        .then(response => {

            console.log(response)
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
        .then(response => response.text())
        .then(data => {
            $(".cart-block").html(data)
        })
})