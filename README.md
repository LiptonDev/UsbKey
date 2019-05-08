# UsbKey

![Example](anim.gif)

## What is the UsbKey?

UsbKey - program for blocking PC by USB.

## How it work?

If the PC is locked and waiting to log in to the session, and it does not have a special USB-key with the key, then when you log in to the session, it is locked again.

## What is the KEY?

Key - is the bytes.  
Key contains:
1) 16 bytes - md5(your data)
2) 16 random bytes

## May I use the USB after create key on USB?

Yes. The key only needs 32 bytes of memory.

## May I use 1 USB-Key for any PC?

Yes. Just use 1 usb.key file in all PC (You can copy this file from USB).
