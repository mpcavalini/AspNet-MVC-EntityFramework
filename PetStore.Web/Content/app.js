function deletePet(event) {
    event.preventDefault();
    event.stopPropagation();

    let id = $(this).data("val-id");
    window.location.href = "/Default/Delete/" + id;
}

function deletePetCancel(event) {
    event.preventDefault();
    event.stopPropagation();

    $('.btn-delete-confirm').removeAttr("data-val-id");
}


$(document).ready(function () {
    $('.btn-delete-confirm').on('click', deletePet);
    $('.btn-delete-cancel').on('click', deletePetCancel);
    $('#deleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var modal = $(this);

        modal.find('.modal-body > p').text(button.data("val-name"));
        modal.find('.btn-delete-confirm').attr("data-val-id",button.data("val-id"));
        
    });
});