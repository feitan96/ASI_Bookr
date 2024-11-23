let dpicn = document.querySelector(".dpicn");
let dropdown = document.querySelector(".dropdown");

if (dpicn) {
    dpicn.addEventListener("click", () => {
        dropdown.classList.toggle("dropdown-open");
    })
}