delete android-armv7-debug.apk
delete android-debug.apk
copy ..\App\carIn\platforms\android\build\outputs\apk\android-debug.apk carIn.apk
copy ..\App\userCarIn\platforms\android\build\outputs\apk\android-debug-unaligned.apk userCarIn.apk
rename carIn.apk 云乐享车-业务员.apk
rename userCarIn.apk 云乐享车-用户端.apk