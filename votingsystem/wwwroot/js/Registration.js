document.addEventListener("DOMContentLoaded", function () {
   
    let button = document.querySelector(".button input");
    button.addEventListener("mouseover", () => {
        button.style.transform = "scale(1.05)";
    });
    button.addEventListener("mouseout", () => {
        button.style.transform = "scale(1)";
    });

    
    let inputs = document.querySelectorAll("input, select");
    inputs.forEach(input => {
        input.addEventListener("focus", () => {
            input.style.borderColor = "#16a085";
        });
        input.addEventListener("blur", () => {
            input.style.borderColor = "#ccc";
        });
    });

    // Auto-hide message box
    //const messageBox = document.getElementById("messageBox");
    //if (messageBox) {
    //    console.log("Message box found:", messageBox.textContent);
    //    setTimeout(() => {
    //        messageBox.style.display = "none";
    //        console.log("Message box hidden.");
    //    }, 5000);
    //} else {
    //    console.log("No message box found.");
    //}
});
