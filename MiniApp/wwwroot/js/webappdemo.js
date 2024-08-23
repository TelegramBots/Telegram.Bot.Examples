var DemoApp = {
  initData: Telegram.WebApp.initData || '',
  initDataUnsafe: Telegram.WebApp.initDataUnsafe || {},
  MainButton: Telegram.WebApp.MainButton,
  BackButton: Telegram.WebApp.BackButton,
  SettingsButton: Telegram.WebApp.SettingsButton,

  init: function(options) {
    $('body').css('visibility', '');
    Telegram.WebApp.ready();
    Telegram.WebApp.MainButton.setParams({
      text: 'CLOSE WEBVIEW',
      is_visible: true
    }).onClick(DemoApp.close);
    Telegram.WebApp.BackButton.onClick(function() {
      DemoApp.showAlert('Back button pressed');
    });
    Telegram.WebApp.SettingsButton.onClick(function() {
      DemoApp.showAlert('Settings opened!');
    });
  },
  expand: function() {
    Telegram.WebApp.expand();
  },
  close: function() {
    Telegram.WebApp.close();
  },
  sendMessage: function(msg_id, with_webview) {
    if (!DemoApp.initDataUnsafe.query_id) {
      alert('WebViewQueryId not defined');
      return;
    }
    $('button').prop('disabled', true);
    $('#btn_status').text('Sending...').removeClass('ok err').show();
    DemoApp.apiRequest('sendMessage', {
      msg_id: msg_id || '',
      with_webview: !DemoApp.initDataUnsafe.receiver && with_webview ? 1 : 0
    }, function(result) {
      $('button').prop('disabled', false);
      if (result.response) {
        if (result.response.ok) {
          $('#btn_status').text('Message sent successfully!').addClass('ok').show();
        } else {
          $('#btn_status').text(result.response.description).addClass('err').show();
          alert(result.response.description);
        }
      } else if (result.error) {
        $('#btn_status').text(result.error).addClass('err').show();
        alert(result.error);
      } else {
        $('#btn_status').text('Unknown error').addClass('err').show();
        alert('Unknown error');
      }
    });
  },
  changeMenuButton: function(close) {
    $('button').prop('disabled', true);
    $('#btn_status').text('Changing button...').removeClass('ok err').show();
    DemoApp.apiRequest('changeMenuButton', {}, function(result) {
      $('button').prop('disabled', false);
      if (result.response) {
        if (result.response.ok) {
          $('#btn_status').text('Button changed!').addClass('ok').show();
          Telegram.WebApp.close();
        } else {
          $('#btn_status').text(result.response.description).addClass('err').show();
          alert(result.response.description);
        }
      } else if (result.error) {
        $('#btn_status').text(result.error).addClass('err').show();
        alert(result.error);
      } else {
        $('#btn_status').text('Unknown error').addClass('err').show();
        alert('Unknown error');
      }
    });
    if (close) {
      setTimeout(function() {
        Telegram.WebApp.close();
      }, 50);
    }
  },
  checkInitData: function() {
    if (DemoApp.initDataUnsafe.query_id &&
        DemoApp.initData &&
        $('#webview_data_status').hasClass('status_need')) {
      $('#webview_data_status').removeClass('status_need');
      DemoApp.apiRequest('checkInitData', {}, function(result) {
        if (result.ok) {
          $('#webview_data_status').text('Hash is correct (async)').addClass('ok');
        } else {
          $('#webview_data_status').text(result.error + ' (async)').addClass('err');
        }
      });
    }
  },

  sendText: function(spam) {
    var text = $('#text_field').val();
    if (!text.length) {
      return $('#text_field').focus();
    }
    if (byteLength(text) > 4096) {
      return alert('Text is too long');
    }
    var repeat = spam ? 10 : 1;
    for (var i = 0; i < repeat; i++) {
      Telegram.WebApp.sendData(text);
    }
  },
  sendTime: function(spam) {
    var repeat = spam ? 10 : 1;
    for (var i = 0; i < repeat; i++) {
      Telegram.WebApp.sendData(new Date().toString());
    }
  },
  switchInlineQuery: function(query, choose_chat) {
    var choose_chat_types = false;
    if (choose_chat) {
      var choose_chat_types = [];
      var types = ['users', 'bots', 'groups', 'channels'];
      for (var i = 0; i < types.length; i++) {
        if ($('#select-' + types[i]).prop('checked')) {
          choose_chat_types.push(types[i]);
        }
      }
      if (!choose_chat_types.length) {
        return DemoApp.showAlert('Select chat types!');
      }
    }
    Telegram.WebApp.switchInlineQuery(query, choose_chat_types);
  },
  requestLocation: function(el) {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(function (position) {
        $(el).next('span').text('(' + position.coords.latitude + ', ' + position.coords.longitude + ')').attr('class', 'ok');
      });
    } else {
      $(el).next('span').text('Geolocation is not supported in this browser.').attr('class', 'err');
    }
    return false;
  },
  requestVideo: function(el) {
    if (navigator.mediaDevices) {
      navigator.mediaDevices.getUserMedia({ audio: false, video: true }).then(function(stream) {
        $(el).next('span').text('(Access granted)').attr('class', 'ok');
      });
    } else {
      $(el).next('span').text('Media devices is not supported in this browser.').attr('class', 'err');
    }
    return false;
  },
  requestAudio: function(el) {
    if (navigator.mediaDevices) {
      navigator.mediaDevices.getUserMedia({ audio: true, video: false }).then(function(stream) {
        $(el).next('span').text('(Access granted)').attr('class', 'ok');
      });
    } else {
      $(el).next('span').text('Media devices is not supported in this browser.').attr('class', 'err');
    }
    return false;
  },
  requestAudioVideo: function(el) {
    if (navigator.mediaDevices) {
      navigator.mediaDevices.getUserMedia({ audio: true, video: true }).then(function(stream) {
        $(el).next('span').text('(Access granted)').attr('class', 'ok');
      });
    } else {
      $(el).next('span').text('Media devices is not supported in this browser.').attr('class', 'err');
    }
    return false;
  },
  testClipboard: function(el) {
    Telegram.WebApp.readTextFromClipboard(function(clipText) {
      if (clipText === null) {
        $(el).next('span').text('Clipboard text unavailable.').attr('class', 'err');
      } else {
        $(el).next('span').text('(Read from clipboard: «' + clipText + '»)').attr('class', 'ok');
      }
    });
    return false;
  },
  requestWriteAccess: function(el) {
    Telegram.WebApp.requestWriteAccess(function(allowed) {
      if (allowed) {
        $(el).next('span').text('(Access granted)').attr('class', 'ok');
      } else {
        $(el).next('span').text('(User declined this request)').attr('class', 'err');
      }
    });
  },
  requestPhoneNumber: function(el) {
    Telegram.WebApp.requestContact(function(sent, event) {
      if (sent) {
        $(el).next('span').text('(Phone number sent to the bot' + (event && event.responseUnsafe && event.responseUnsafe.contact && event.responseUnsafe.contact.phone_number ? ': +' + event.responseUnsafe.contact.phone_number : '') + ')').attr('class', 'ok');
      } else {
        $(el).next('span').text('(User declined this request)').attr('class', 'err');
      }
    });
  },
  requestServerTime: function(el) {
    Telegram.WebApp.invokeCustomMethod('getCurrentTime', {}, function(err, time) {
      if (err) {
        $(el).next('span').text('(' + err + ')').attr('class', 'err');
      } else {
        $(el).next('span').text('(' + (new Date(time*1000)).toString() + ')').attr('class', 'ok');
      }
    });
  },
  cloudStorageKeys: {},
  cloudStorageItems: {},
  editCloudRow: function(el, event) {
    event.preventDefault();
    var values = DemoApp.cloudStorageItems;
    var key = $(el).parents('tr').attr('data-key');
    el.form.reset();
    el.form.key.value = key;
    el.form.value.value = values[key];
  },
  deleteCloudRow: function(el, event) {
    event.preventDefault();
    var key = $(el).parents('tr').attr('data-key');
    Telegram.WebApp.CloudStorage.removeItem(key, function(err, deleted) {
      if (err) {
        DemoApp.showAlert('Error: ' + err);
      } else {
        if (deleted) {
          var index = DemoApp.cloudStorageKeys.indexOf(key);
          if (index >= 0) {
            DemoApp.cloudStorageKeys.splice(index, 1);
          }
          delete DemoApp.cloudStorageItems[key];
        }
        el.form.reset();
        DemoApp.updateCloudRows();
      }
    });
  },
  saveCloudForm: function(form, event) {
    event.preventDefault();
    var key = form.key.value;
    var value = form.value.value;
    Telegram.WebApp.CloudStorage.setItem(key, value, function(err, saved) {
      if (err) {
        DemoApp.showAlert('Error: ' + err);
      } else {
        if (saved) {
          if (typeof DemoApp.cloudStorageItems[key] === 'undefined') {
            DemoApp.cloudStorageKeys.push(key);
          }
          DemoApp.cloudStorageItems[key] = value;
        }
        form.reset();
        DemoApp.updateCloudRows();
      }
    });
  },
  updateCloudRows: function() {
    var html = '';
    var keys = DemoApp.cloudStorageKeys;
    var values = DemoApp.cloudStorageItems;
    for (var i = 0; i < keys.length; i++) {
      var key = keys[i];
      html += '<tr data-key="'+cleanHTML(key)+'"><td>'+cleanHTML(key)+'</td><td>'+cleanHTML(values[key])+'</td><td><button onclick="DemoApp.editCloudRow(this, event);">Edit</button><button onclick="DemoApp.deleteCloudRow(this, event);">Delete</button></td></tr>';
    }
    $('#cloud_rows').html(html);
  },
  loadCloudKeys: function(el) {
    Telegram.WebApp.CloudStorage.getKeys(function(err, keys) {
      if (err) {
        DemoApp.showAlert('Error: ' + err);
      } else {
        if (keys.length > 0) {
          Telegram.WebApp.CloudStorage.getItems(keys, function(err, values) {
            if (err) {
              DemoApp.showAlert('Error: ' + err);
            } else {
              DemoApp.cloudStorageKeys = keys;
              DemoApp.cloudStorageItems = {};
              for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                DemoApp.cloudStorageItems[key] = values[key];
              }
              DemoApp.updateCloudRows();
            }
          });
        }
      }
    });
  },
  biometricInit: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!DemoApp.biometricInited) {
      DemoApp.biometricInited = true;
      Telegram.WebApp.onEvent('biometricManagerUpdated', function() {
        $('#bm_inited').text(biometricManager.isInited ? 'true' : 'false');
        $('#bm_available').text(biometricManager.isBiometricAvailable ? 'true' : 'false');
        $('#bm_type').text(biometricManager.biometricType || '');
        $('#bm_access_requested').text(biometricManager.isAccessRequested ? 'true' : 'false');
        $('#bm_access_granted').text(biometricManager.isAccessGranted ? 'true' : 'false');
        $('#bm_token_saved').text(biometricManager.isBiometricTokenSaved ? 'true' : 'false');
        $('#bm_device_id').text(biometricManager.deviceId || '');
        $('#bm_settings').toggle(biometricManager.isBiometricAvailable && biometricManager.isAccessRequested && !biometricManager.isAccessGranted);
      });
    }
    biometricManager.init();
  },
  biometricRequestAccess: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!biometricManager.isInited) {
      return DemoApp.showAlert('Biometric not inited yet!');
    }
    biometricManager.requestAccess({reason: 'The bot uses biometrics for testing purposes.'}, function(access_granted) {
      if (access_granted) {
        $(el).next('span').text('(Access granted)').attr('class', 'ok');
      } else {
        $(el).next('span').text('(Request declined)').attr('class', 'err');
      }
    });
  },
  biometricRequestAuth: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!biometricManager.isInited) {
      return DemoApp.showAlert('Biometric not inited yet!');
    }
    $(el).next('span').text('').attr('class', '');
    biometricManager.authenticate({reason: 'The bot requests biometrics for testing purposes.'}, function(success, token) {
      if (success) {
        $(el).next('span').text('(Success, token: ' + token + ')').attr('class', 'ok');
      } else {
        $(el).next('span').text('(Auth failed)').attr('class', 'err');
      }
    });
  },
  biometricOpenSettings: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!biometricManager.isInited) {
      return DemoApp.showAlert('Biometric not inited yet!');
    }
    if (!biometricManager.isBiometricAvailable ||
        !biometricManager.isAccessRequested ||
        biometricManager.isAccessGranted) {
      return false;
    }
    biometricManager.openSettings();
  },
  biometricSetToken: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!biometricManager.isInited) {
      return DemoApp.showAlert('Biometric not inited yet!');
    }
    var token = parseInt(Math.random().toString().substr(2)).toString(16);
    biometricManager.updateBiometricToken(token, function(updated) {
      if (updated) {
        $('#bm_token_saved').text(biometricManager.isBiometricTokenSaved ? 'true' : 'false');
        $(el).nextAll('span').text('(Updated: ' + token + ')').attr('class', 'ok');
      } else {
        $(el).next('span').text('(Failed)').attr('class', 'err');
      }
    });
  },
  biometricRemoveToken: function(el) {
    var biometricManager = Telegram.WebApp.BiometricManager;
    if (!biometricManager.isInited) {
      return DemoApp.showAlert('Biometric not inited yet!');
    }
    biometricManager.updateBiometricToken('', function(updated) {
      if (updated) {
        $('#bm_token_saved').text(biometricManager.isBiometricTokenSaved ? 'true' : 'false');
        $(el).next('span').text('(Removed)').attr('class', 'ok');
      } else {
        $(el).next('span').text('(Failed)').attr('class', 'err');
      }
    });
  },
  toggleMainButton: function(el) {
    if (DemoApp.MainButton.isVisible) {
      DemoApp.MainButton.hide();
      el.innerHTML = 'Show Main Button';
    } else {
      DemoApp.MainButton.show();
      el.innerHTML = 'Hide Main Button';
    }
  },
  toggleBackButton: function(el) {
    if (DemoApp.BackButton.isVisible) {
      DemoApp.BackButton.hide();
      el.innerHTML = 'Show Back Button';
    } else {
      DemoApp.BackButton.show();
      el.innerHTML = 'Hide Back Button';
    }
  },
  toggleSettingsButton: function(el) {
    if (DemoApp.SettingsButton.isVisible) {
      DemoApp.SettingsButton.hide();
      el.innerHTML = 'Show Settings Button';
    } else {
      DemoApp.SettingsButton.show();
      el.innerHTML = 'Hide Settings Button';
    }
  },
  toggleSwipeBehavior: function(el) {
    if (Telegram.WebApp.isVerticalSwipesEnabled) {
      Telegram.WebApp.disableVerticalSwipes();
      el.innerHTML = 'Enable Vertical Swypes';
    } else {
      Telegram.WebApp.enableVerticalSwipes();
      el.innerHTML = 'Disable Vertical Swypes';
    }
  },
  showAlert: function(message) {
    Telegram.WebApp.showAlert(message);
  },
  showConfirm: function(message) {
    Telegram.WebApp.showConfirm(message);
  },
  showPopup: function() {
    Telegram.WebApp.showPopup({
      title: 'Popup title',
      message: 'Popup message',
      buttons: [
        {id: 'delete', type: 'destructive', text: 'Delete all'},
        {id: 'faq', type: 'default', text: 'Open FAQ'},
        {type: 'cancel'},
      ]
    }, function(button_id) {
      if (button_id == 'delete') {
        DemoApp.showAlert("'Delete all' selected");
      } else if (button_id == 'faq') {
        Telegram.WebApp.openLink('https://telegram.org/faq');
      }
    });
  },
  showScanQrPopup: function(links_only) {
    Telegram.WebApp.showScanQrPopup({
      text: links_only ? 'with any link' : 'for test purposes'
    }, function(text) {
      if (links_only) {
        var lower_text = text.toString().toLowerCase();
        if (lower_text.substr(0, 7) == 'http://' ||
            lower_text.substr(0, 8) == 'https://') {
          setTimeout(function() {
            Telegram.WebApp.openLink(text);
          }, 50);
          return true;
        }
      } else {
        DemoApp.showAlert(text);
        return true;
      }
    });
  },

  apiRequest: function(method, data, onCallback) {
    var authData = DemoApp.initData || '';
    $.ajax(DemoApp.apiUrl, {
      type: 'POST',
      data: $.extend(data, {_auth: authData, method: method}),
      dataType: 'json',
      xhrFields: {
        withCredentials: true
      },
      success: function(result) {
        onCallback && onCallback(result);
      },
      error: function(xhr) {
        onCallback && onCallback({error: 'Server error'});
      }
    });
  }
};

var DemoAppMenu = {
  init: function() {
    DemoApp.init();
    $('body').addClass('gray');
    Telegram.WebApp.setHeaderColor('secondary_bg_color');
  }
};

var DemoAppInitData = {
  init: function() {
    DemoApp.init();
    // $('body').addClass('gray');
    // Telegram.WebApp.setHeaderColor('secondary_bg_color');

    Telegram.WebApp.onEvent('themeChanged', function() {
      $('#theme_data').text(JSON.stringify(Telegram.WebApp.themeParams, null, 2));
    });
    $('#webview_data').text(JSON.stringify(DemoApp.initDataUnsafe, null, 2));
    $('#theme_data').text(JSON.stringify(Telegram.WebApp.themeParams, null, 2));
    DemoApp.checkInitData();

  }
};

var DemoAppViewport = {
  init: function() {
    DemoApp.init();
    // $('body').addClass('gray');
    // Telegram.WebApp.setHeaderColor('secondary_bg_color');
    Telegram.WebApp.onEvent('viewportChanged', DemoAppViewport.setData);
    DemoAppViewport.setData();
  },
  setData: function() {
    $('.viewport-border').attr('text', window.innerWidth + ' x ' + round(Telegram.WebApp.viewportHeight, 2));
    $('.viewport-stable_border').attr('text', window.innerWidth + ' x ' + round(Telegram.WebApp.viewportStableHeight, 2) + ' | is_expanded: ' + (Telegram.WebApp.isExpanded ? 'true' : 'false'));
  }
};

function cleanHTML(value) {
  return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/\n/g, '<br/>');
}

function byteLength(str) {
  if (window.Blob) {
    try { return new Blob([str]).size; } catch (e) {}
  }
  var s = str.length;
  for (var i=str.length-1; i>=0; i--) {
    var code = str.charCodeAt(i);
    if (code > 0x7f && code <= 0x7ff) s++;
    else if (code > 0x7ff && code <= 0xffff) s+=2;
    if (code >= 0xDC00 && code <= 0xDFFF) i--;
  }
  return s;
}

function round(val, d) {
  var k = Math.pow(10, d || 0);
  return Math.round(val * k) / k;
}

(function($) {
  $.fn.cssProp = function(prop, val) {
    if (typeof val !== 'undefined') {
      return this.each(function() {
        if (this.style && this.style.setProperty) {
          this.style.setProperty(prop, val);
        }
      });
    }
    return this.first().map(function() {
      if (this.style && this.style.getPropertyValue) {
        return this.style.getPropertyValue(prop);
      } else {
        return '';
      }
    }).get(0) || '';
  };
})(jQuery);