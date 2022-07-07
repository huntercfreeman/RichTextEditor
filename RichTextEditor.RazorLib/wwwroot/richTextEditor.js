window.richTextEditor = {
  intersectionObserver: 0,
  elementByIdIsIntersecting: new Map(),
  getActiveRowId: function (richTextEditorGuid) {
    return `rte_active-row_${richTextEditorGuid}`;
  },
  getRichTextEditorId: function (richTextEditorGuid) {
    return `rte_rich-text-editor-display_${richTextEditorGuid}`;
  },
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
  scrollIntoViewIfOutOfViewport: function (inputElementReference) {
    const elementPosition = inputElementReference.getBoundingClientRect().top;

    const value = this.elementByIdIsIntersecting.get(inputElementReference.id);
    const activeRow = document.getElementById(this.getActiveRowId(value.richTextEditorGuid));
    const offsetPosition = elementPosition - activeRow.offsetHeight;

    let richTextEditorDisplay = document.getElementById(this.getRichTextEditorId(value.richTextEditorGuid));

    if (richTextEditorDisplay.scrollTop < activeRow.offsetTop) {
      richTextEditorDisplay.scrollTop = activeRow.offsetTop + activeRow.offsetHeight;
    }
    else {
      richTextEditorDisplay.scrollTop = activeRow.offsetTop - activeRow.offsetHeight;
    }
    
    // richTextEditorDisplay.scrollBy({
    //      top: offsetPosition,
    //      behavior: "smooth"
    // });
  },
  initializeIntersectionObserver: function () {
    let options = {
        rootMargin: '0px',
        threshold: 1
    }

    this.intersectionObserver = new IntersectionObserver((entries) => this.handleThresholdChange(entries, this.elementByIdIsIntersecting), options);
  },
  subscribeScrollIntoView: function (elementId, richTextEditorGuid) {
    this.elementByIdIsIntersecting.set(elementId, {
      intersectionRatio: 0,
      richTextEditorGuid: richTextEditorGuid
    });

    let element = document.getElementById(elementId);
    this.intersectionObserver.observe(element);
  },
  disposeScrollIntoView: function (elementId) {
    let element = document.getElementById(elementId);
    this.intersectionObserver.unobserve(element);
  },
  handleThresholdChange: function (entries, elementByIdIsIntersecting) {
    for (let i = 0; i < entries.length; i++) {
        let currentEntry = entries[i];

        let previousValue = elementByIdIsIntersecting.get(currentEntry.target.id);

        elementByIdIsIntersecting.set(currentEntry.target.id, {
          intersectionRatio: currentEntry.intersectionRatio,
          richTextEditorGuid: previousValue.richTextEditorGuid
        });
    }
  }
};
