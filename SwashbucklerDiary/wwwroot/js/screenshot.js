﻿import '/npm/html2canvas/1.4.1/dist/html2canvas.min.js';
export function getScreenshotBase64(dotNetCallbackRef, callbackMethod, element) {
    return new Promise((resolve, reject) => {
        html2canvas(document.querySelector(element), {
            allowTaint: true,
            onclone: (cloned) => {
                Array.from(cloned.querySelectorAll('textarea')).forEach((textArea) => {
                    const div = cloned.createElement('div')
                    div.innerText = textArea.value
                    textArea.style.display = 'none'
                    textArea.parentElement.append(div)
                });
                return convertAllImagesToBase64(cloned);
            },
        }).then(canvas => {
            resolve(canvas.toDataURL())
        });
    });

    //修复截图不加载跨域图片
    //https://github.com/niklasvh/html2canvas/issues/1614#issuecomment-468452767
    function convertAllImagesToBase64(cloned) {
        const pendingImagesPromises = [];
        const pendingPromisesData = [];

        const images = cloned.getElementsByTagName('img');

        for (let i = 0; i < images.length; i += 1) {
            // First we create an empty promise for each image
            const promise = new Promise((resolve, reject) => {
                pendingPromisesData.push({
                    index: i, resolve, reject,
                });
            });
            // We save the promise for later resolve them
            pendingImagesPromises.push(promise);
        }

        for (let i = 0; i < images.length; i += 1) {
            // We fetch the current image
            dotNetCallbackRef.invokeMethodAsync(callbackMethod, images[i].getAttribute('src'))
                .then((data) => {
                    const pending = pendingPromisesData.find((p) => p.index === i);
                    images[i].src = data;
                    pending.resolve(data);
                })
                .catch((e) => {
                    const pending = pendingPromisesData.find((p) => p.index === i);
                    pending.reject(e);
                });
        }

        // This will resolve only when all the promises resolve
        return Promise.all(pendingImagesPromises);
    };
}