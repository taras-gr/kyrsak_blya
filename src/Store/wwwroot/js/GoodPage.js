
window.addEventListener("load", function () {
    document.getElementById("reviews").style.display = "none";
});

var isHidden = true;

function ShowReviews() {
    if (!isHidden) {
        document.getElementById("reviews").style.display = "none";
        isHidden = true;
    }
    else {
        document.getElementById("reviews").style.display = "block";
        isHidden = false;
    }
}

var starId = ["mark-one", "mark-two", "mark-three", "mark-four", "mark-five"]
var mark = 0;

function Star(i) {
    for (index = 0; index < starId.length; index++) {
        document.getElementById(starId[index]).style.color = "#fff";
    }

    for (index = 0; index < i; index++) {
        document.getElementById(starId[index]).style.color = "#ffdb11";
    }

    mark = i;
    document.getElementById("mark").value = mark;
}

var isEditing = false;

function Edit(id) {
    if (!isEditing) {
        document.getElementById("editButton" + id.toString()).style.display = "none";
        document.getElementById("saveButton" + id.toString()).style.display = "inline-block";

        document.getElementById("reviewMessage" + id.toString()).style.display = "none";
        document.getElementById("hiddenInput" + id.toString()).style.display = "block";

        document.getElementById("starCount" + id.toString()).style.display = "none";
        document.getElementById("hiddenStarCount" + id.toString()).style.display = "block";

        isEditing = true;
    }
}