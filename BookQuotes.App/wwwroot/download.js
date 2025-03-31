function downloadFile(fileName, base64, contentType) {
    const link = document.createElement("a");
    const blob = new Blob([Uint8Array.from(atob(base64), c => c.charCodeAt(0))], { type: contentType });
    const url = URL.createObjectURL(blob);

    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url);
}
