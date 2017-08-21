# SwapMouseButtons
This is a command line utility to switch primary and secondary mouse buttons. I regularly
switch sides with my mouse to try and avoid repetitive posture related issues while working
in front of the box. This gave me a build window command line story for switching mouse 
buttons without having to navigate to the mouse properties gui.   

It uses a registry lookup to determine what mouse buttons setting is currently in place then
a managed code api wrapper call to the win32 SwapMouseButton api to switch the setting to the
opposite of whatever it currently is.

reg query "hkcu\Control Panel\Mouse" /v SwapMouseButtons 
- reg_sz = 0 = right handed mouse buttons setting is enabled
- reg_sz = 1 = left handed mouse buttons setting is enabled

reg add "hkcu\Control Panel\Mouse" /v SwapMouseButtons /t reg_sz /d 0|1
- this doesn't apply setting until you signout and back in again presumably 
because that triggers mouse button settings lookup and initialization

Calling SwapMouseButton win32 api changes what is currently in effect but does
not change the registry setting. So that needs to be modified as well otherwise
the next time you open and close mouse settings control panel, main.cpl, the
setting will revert to what the registry setting is configured to be.

Testing showed that for user logged in who was member of administrators
group only needed to open registry key with writable enabled and only
needed user access control app.manifest run elevanted permissions, which
generates yes/no popup, to address use of utility by user who is not 
member of administrators security group. So leaving that behavior out 
for usability purposes, i.e. so cli utility operates w/o any popups.

"addressing registry UnauthorizedAccessException" -> 
https://stackoverflow.com/questions/10339990/c-sharp-set-registry-value-throws-unauthorizedaccessexception ->
https://stackoverflow.com/questions/2732126/deletesubkey-unauthorizedaccessexception | OpenSubKey(keyPath, true)) // open writable

"windows uac elevation request api" -> https://stackoverflow.com/questions/6418791/requesting-administrator-privileges-at-run-time | 
https://msdn.microsoft.com/en-us/library/bb756929.aspx step 6: create and embed an application manifest (uac) | managed code | 
enable following post-build event command line as it seems using build action = "embedded resource" or "resource" doesn't do it
"%programfiles(x86)%\Windows Kits\10\bin\10.0.15063.0\x86\mt.exe" -manifest "$(ProjectDir)App.manifest" 
-outputresource:"$(TargetDir)$(TargetName).exe;#1"
https://stackoverflow.com/questions/17533/request-windows-vista-uac-elevation-if-path-is-protected | uac sample -> 
https://msdn.microsoft.com/en-us/library/aa970890.aspx

&nbsp;  

---
# References
1. "switch mouse buttons command line utility" ->
- https://stackoverflow.com/questions/4806575/how-do-i-use-rundll32-to-swapmousebutton
- https://superuser.com/questions/857259/how-do-you-swap-the-primary-mouse-button-via-commandline-in-windows-8-without-a
- https://www.codeproject.com/Articles/66272/Programmatically-Swapping-Mouse-Buttons
  
2. "c# read registry key value" -> 
- https://stackoverflow.com/questions/18232972/how-to-read-value-of-a-registry-key-c-sharp  

3. "SwapMouseButton win32 api" -> 
- https://msdn.microsoft.com/en-us/library/ms646264(VS.85).aspx  

4. "windows user access control elevation request api" -> 
- https://stackoverflow.com/questions/17533/request-windows-vista-uac-elevation-if-path-is-protected
- https://www.codeproject.com/Articles/19165/Vista-UAC-The-Definitive-Guide

5. "trustInfo security requestedPrivileges requestedExecutionLevel requireAdministrator" ->
- https://stackoverflow.com/questions/6226976/how-to-add-manifest-requestedprivileges-info-into-delphi-project
 
6. "which open source license should i use" -> 
- https://whichopensourcelicense.com/  
