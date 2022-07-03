window.blazorStudioComponents = {
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
  }
};
