async function send() {
    const message = document.getElementById("msg").value;

    const res = await fetch("/Chat/Ask", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            question: message
        })
    });

    const data = await res.json();

    const reply =
        typeof data.answer === "string"
            ? data.answer
            : (data.detail != null ? JSON.stringify(data.detail) : "Request failed.");

    document.getElementById("chatBox").innerHTML +=
        `<p><b>You:</b> ${message}</p>
     <p><b>AI:</b> ${reply}</p>`;
}