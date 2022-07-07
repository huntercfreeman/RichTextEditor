window.richTextEditor = {
  intersectionObserver: 0,
  dotNetObjectReferences: new Map(),
  initOnKeyDownProvider: function (onKeyDownProviderDisplayReference) {
      document.addEventListener('keydown', (e) => {
          if (e.key === "Tab") {
              e.preventDefault();
          }
          if (e.key === "a" && e.ctrlKey) {
              e.preventDefault();
          }

          let dto = {
              "key": e.key,
              "code": e.code,
              "ctrlWasPressed": e.ctrlKey,
              "shiftWasPressed": e.shiftKey,
              "altWasPressed": e.altKey
          };

          onKeyDownProviderDisplayReference.invokeMethodAsync('FireOnKeyDownEvent', dto);
      });
  },
  clearInputElement: function (inputElementReference) {
    inputElementReference.value = "";
  },
  scrollIntoView: function (inputElementReference) {
    inputElementReference.scrollIntoView();
  },
  initializeIntersectionObserver: function () {
    let options = {
        rootMargin: '0px',
        threshold: 0.25
    }

    this.intersectionObserver = new IntersectionObserver((entries) => this.handleThresholdChange(entries, this.dotNetObjectReferences), options);
  },
  subscribeScrollIntoView: function (elementId, dotNetObjectReference) {
    let element = document.getElementById(elementId);

    this.dotNetObjectReferences.set(elementId, dotNetObjectReference);

    this.intersectionObserver.observe(element);
  },
  disposeScrollIntoView: function (elementId) {
    let element = document.getElementById(elementId);
    this.intersectionObserver.unobserve(element);
  },
  handleThresholdChange: function (entries, dotNetObjectReferences) {
    for (let i = 0; i < entries.length; i++) {
        let currentEntry = entries[i];

        if (!currentEntry.isIntersecting) {
            dotNetObjectReferences.get(currentEntry.target.id)
                .invokeMethodAsync("ScrollIntoViewAsync");
        }
    }
  }
};
