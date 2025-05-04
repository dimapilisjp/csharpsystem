document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".delete-button").forEach(button => {
        button.addEventListener("click", function (event) {
            const confirmDelete = confirm("Are you sure you want to delete this voter?");
            if (!confirmDelete) {
                event.preventDefault(); 
            }
        });
    });
    function confirmDeletion(voterId) {
        return confirm(`Are you sure you want to delete voter with ID ${voterId}?`);
    }
});
