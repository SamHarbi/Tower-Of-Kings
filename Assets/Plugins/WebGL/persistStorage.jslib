    mergeInto(LibraryManager.library, {
        persist: function (data) {
            localStorage.setItem("TowerOfKings_SaveData", data);
        },
    });