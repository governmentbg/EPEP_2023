const dropZone = document.getElementById("drop_zone");
const dropInput = document.getElementById("uploadFile");

let progressBar = document.getElementById("progress-bar");
let uploadProgress = [];

dropZone.addEventListener("drop", dropHandler);
dropInput.addEventListener("change", fileUploadHandler);

//Resets and Hihlights
["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => {
    dropZone.addEventListener(eventName, preventDefaults, false);
    document.body.addEventListener(eventName, preventDefaults, false);
});

[("dragenter", "dragover")].forEach(eventName => {
    dropZone.addEventListener(eventName, highlight, false);
});

["dragleave", "drop"].forEach(eventName => {
    dropZone.addEventListener(eventName, unhighlight, false);
});

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}
function highlight() {
    dropZone.classList.add("drag-and-drop--highlight");
}
function unhighlight() {
    dropZone.classList.contains("drag-and-drop--highlight") ? dropZone.classList.remove("drag-and-drop--highlight") : "";
}

function initializeProgress(numFiles) {
    progressBar.value = 0;
    uploadProgress = [];

    for (let i = numFiles; i > 0; i--) {
        uploadProgress.push(0);
    }
}

function updateProgress(fileNumber, percent) {
    uploadProgress[fileNumber] = percent;
    let total = uploadProgress.reduce((tot, curr) => tot + curr, 0) / uploadProgress.length;
    progressBar.value = total;
}

function dropHandler(e) {
    let dt = e.dataTransfer;
    let files = dt.files;
    initializeProgress(files.length);
    if (files) {
        fileUploadHandler(files);
    }
}

function fileUploadHandler(files, i) {
    var url = "ввв";
    var xhr = new XMLHttpRequest();
    var formData = new FormData();

    xhr.open("POST", url, true);

    xhr.upload.addEventListener("progress", function (e) {
        updateProgress(i, (e.loaded * 100.0) / e.total || 100);
    });

    xhr.addEventListener("readystatechange", function (e) {
        if (xhr.readyState == 4 && xhr.status == 200) {
            updateProgress(i, 100);
        } else if (xhr.readyState == 4 && xhr.status != 200) {
        }
    });

    formData.append("file", files);
    xhr.send(formData);
}
