mergeInto(LibraryManager.library, {
	StarkPointerStringify: function(str) {
		if (typeof UTF8ToString !== "undefined") {
			return UTF8ToString(str)
		}
		return Pointer_stringify(str)
	},
    StarkMigratingData: function() {
        if (!window.StarkSDK) return false;
        return window.StarkSDK.MigratingData();
    },
    StarkIsDataMigrated: function() {
         if (!window.StarkSDK) return false;
         return window.StarkSDK.IsDataMigrated();
    },
    StarkCanUseLocalStorage: function() {
        if (!window.StarkSDK) return false;
        return window.StarkSDK.CanUseLocalStorage();
    },
	StarkStorageSetIntSync: function(key, value) {
		window.StarkSDK && window.StarkSDK.SetStorageSync(_StarkPointerStringify(key), value);
	},
	StarkStorageGetIntSync: function(key, defaultValue) {
		if (!window.StarkSDK) return defaultValue;
		return window.StarkSDK.GetStorageSync(_StarkPointerStringify(key), defaultValue);
	},
	StarkStorageSetFloatSync: function(key, value) {
		window.StarkSDK && window.StarkSDK.SetStorageSync(_StarkPointerStringify(key), value);
	},
	StarkStorageGetFloatSync: function(key, defaultValue) {
		if (!window.StarkSDK) return defaultValue;
		return window.StarkSDK.GetStorageSync(_StarkPointerStringify(key), defaultValue);
	},
	StarkStorageSetStringSync: function(key, value) {
		window.StarkSDK && window.StarkSDK.SetStorageSync(_StarkPointerStringify(key), _StarkPointerStringify(value));
	},
	StarkStorageGetStringSync: function(key, defaultValue) {
		if (!window.StarkSDK) return defaultValue;
		var returnStr = window.StarkSDK.GetStorageSync(_StarkPointerStringify(key), _StarkPointerStringify(defaultValue));
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkStorageDeleteAllSync: function() {
		window.StarkSDK && window.StarkSDK.ClearStorageSync();
	},
	StarkStorageDeleteKeySync: function(key) {
		window.StarkSDK && window.StarkSDK.RemoveStorageSync(_StarkPointerStringify(key));
	},
	StarkStorageHasKeySync: function(key) {
		if (!window.StarkSDK) return;
		return window.StarkSDK.StorageHasKeySync(_StarkPointerStringify(key));
	},
	StarkWriteBinFile: function(filePath, data, dataLength, s, f) {
		window.StarkSDK && window.StarkSDK.WriteFile(
			_StarkPointerStringify(filePath),
			HEAPU8.slice(data, dataLength + data),
			"binary",
			_StarkPointerStringify(s),
			_StarkPointerStringify(f)
		)
	},
	StarkWriteBinFileSync: function(filePath, data, dataLength) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.WriteFileSync(
			_StarkPointerStringify(filePath),
			HEAPU8.slice(data, dataLength + data),
			"binary"
		)
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkWriteStringFile: function(filePath, data, encoding, s, f) {
		window.StarkSDK && window.StarkSDK.WriteFile(
			_StarkPointerStringify(filePath),
			_StarkPointerStringify(data),
			_StarkPointerStringify(encoding),
			_StarkPointerStringify(s),
			_StarkPointerStringify(f)
		)
	},
	StarkWriteStringFileSync: function(filePath, data, encoding) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.WriteFileSync(_StarkPointerStringify(filePath), _StarkPointerStringify(data), _StarkPointerStringify(encoding));
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkReadFile: function(filePath, encoding, callbackId) {
		window.StarkSDK && window.StarkSDK.ReadFile(
			_StarkPointerStringify(filePath),
			_StarkPointerStringify(encoding),
			_StarkPointerStringify(callbackId)
		);
	},
	StarkReadStringFileSync: function(filePath, encoding) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.ReadStringFileSync(_StarkPointerStringify(filePath), _StarkPointerStringify(encoding));
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkReadBinFileSync: function(filePath) {
		if (!window.StarkSDK) return;
		return window.StarkSDK.ReadBinFileSync(
                _StarkPointerStringify(filePath)
            );
	},
	StarkShareFileBuffer: function(offset, callbackId) {
		window.StarkSDK && window.StarkSDK.ShareFileBuffer(
			HEAPU8,
			offset,
			_StarkPointerStringify(callbackId)
		)
	},
	StarkAccessFileSync: function(path) {
		if (!window.StarkSDK) return false;
		return window.StarkSDK.AccessFileSync(_StarkPointerStringify(path));
	},
	StarkAccessFile: function(path, s, f) {
		window.StarkSDK && window.StarkSDK.AccessFile(
			_StarkPointerStringify(path),
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkCopyFileSync: function(srcPath, destPath) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.CopyFileSync(_StarkPointerStringify(srcPath), _StarkPointerStringify(destPath));
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkCopyFile: function(srcPath, destPath, s, f) {
		window.StarkSDK && window.StarkSDK.CopyFile(
			_StarkPointerStringify(srcPath),
			_StarkPointerStringify(destPath),
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkRenameFileSync: function(srcPath, destPath) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.RenameFileSync(_StarkPointerStringify(srcPath), _StarkPointerStringify(destPath));
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkRenameFile: function(srcPath, destPath, s, f) {
		window.StarkSDK && window.StarkSDK.RenameFile(
			_StarkPointerStringify(srcPath),
			_StarkPointerStringify(destPath),
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkUnlinkSync: function(filePath) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.UnlinkSync(_StarkPointerStringify(filePath));
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkUnlink: function(filePath, s, f) {
		window.StarkSDK && window.StarkSDK.Unlink(
			_StarkPointerStringify(filePath),
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkMkdir: function(dirPath, recursive, s, f) {
		window.StarkSDK && window.StarkSDK.Mkdir(
			_StarkPointerStringify(dirPath),
			recursive,
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkMkdirSync: function(dirPath, recursive) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.MkdirSync(_StarkPointerStringify(dirPath), recursive);
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkRmdir: function(dirPath, recursive, s, f) {
		window.StarkSDK && window.StarkSDK.Rmdir(
			_StarkPointerStringify(dirPath),
			recursive,
			_StarkPointerStringify(s),
			_StarkPointerStringify(f));
	},
	StarkRmdirSync: function(dirPath, recursive) {
		if (!window.StarkSDK) return;
		var returnStr = window.StarkSDK.RmdirSync(_StarkPointerStringify(dirPath), recursive);
		if (!returnStr) {
			return;
		}
		var bufferSize = lengthBytesUTF8(returnStr) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(returnStr, buffer, bufferSize);
		return buffer;
	},
	StarkStatSync: function(path) {
        if (!window.StarkSDK) return;
        var returnStr = window.StarkSDK.StatSync(_StarkPointerStringify(path));
        if (!returnStr) {
            return;
        }
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
	},
    StarkStat: function(path, callbackId) {
        window.StarkSDK && window.StarkSDK.Stat(
            _StarkPointerStringify(path),
            _StarkPointerStringify(callbackId));
    },
    StarkGetSavedFileList: function(callbackId) {
        window.StarkSDK && window.StarkSDK.GetSavedFileList(
            _StarkPointerStringify(callbackId));
    },
    StarkGetCachedPathForUrl: function(url) {
        if (!window.StarkSDK) return;
        var returnStr = window.StarkSDK.getCachedPathForUrl(_StarkPointerStringify(url));
        if (!returnStr) {
            return;
        }
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});