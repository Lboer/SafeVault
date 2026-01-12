Access admin page via the following script:

fetch("/admin", {
    headers: {
        "Authorization": "Bearer {token}"
    }
})
.then(response => response.text())
.then(html => {
    document.body.innerHTML = html;
}); 