@ECHO OFF

GOTO %1%

:MOUNT
imdisk -a -t file -f C:\temp\ramdisk.img -s 100M -o rw -m O:\
GOTO EXIT

:UNMOUNT
imdisk -D -m O:\

:EXIT
