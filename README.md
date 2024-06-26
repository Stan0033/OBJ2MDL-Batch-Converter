This program will convert dropped .obj (wavefront) files onto it, into .mdl (warcraft 3) and put them in the same folder as the source.

Notice:
1. no matter how much objects you had in your obj file, the outcome will be a single geoset 
2. do not mess with the .obj file with notepad or similar.
3. The outputted geoset will be sideways due to differences in how standard 3D coordiantes work and how openGL coordinates work. You can rotate it after however.
4. 
Bonus to converted mdl file:
- origin ref
- geoset attached to bone "base", and using material
- material with layer using texture "Textures\white.blp"

Requires .net 3.5.

Single .exe file.

![alt text](https://i.ibb.co/CBPrxzN/Screenshot-2024-06-18-052810.png)
