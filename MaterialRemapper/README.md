Material Remapper
=========================
This tool is used to accelerate the material assignment for models containing a lot of meshes (Alembic, etc). This tool works with Legacy as well as SRP pipelines.

The tool was coded in large part by Mike Wutherick.

How to Use
----------------------
* Open the the Remapper Window: Window->Film-TV toolbox->Material Remapper
* Select the root GameObject who's MeshRenderer you want to alter
* Click on "Update Scene Selection". The window will contain the list of unique MeshRenderers: ![import settings](Documentation/WindowPopulated.png)
* Add more materials to the session library by clicking "Add new Material Entry": each new entry is initialized to the default material for the current pipeline.
* Replace the default materials with other materials created in the project.
* Chose material assignments from the popup (Multiple materials per MeshRenderer are supported). 
* Once all assignments are configured, commit the changes to the Scene pressing "Apply Material Changes".
* If at any moment you wish to return to the default state of the Material remapper, press ResetRemapper.
