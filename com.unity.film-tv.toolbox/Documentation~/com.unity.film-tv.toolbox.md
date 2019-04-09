Unity Film / TV toolbox
=========================

This repository is a collection of experimental, proof of concept or helper tools for Film / TV productions.
 

Material Remapper
=========================
Material Remapper allows to more easily assign material to models containing a lot of meshes (Alembic, etc).
It works with Legacy as well as SRP pipelines.

The tool was coded in large part by Mike Wutherick.

How to Use
----------------------
* Open the Material Remapper Window via Window->Film-TV toolbox->Material Remapper
* Select the root GameObject containing the MeshRenderers you want to set-up
* Click on "Update Scene Selection". This will load the list of unique MeshRenderers: ![import settings](images/WindowPopulated.png)
* Add more materials to the session library by clicking "Add new Material Entry". Each new entry is initialized to the default current pipeline material.
* Replace the default materials with the wanted materials.
* Chose material assignments from the popup (Multiple materials per MeshRenderer are supported). 
* Once all assignments are configured, commit the changes to the Scene pressing "Apply Material Changes".
* If at any moment you wish to return to the default state of the Material Remapper, press ResetRemapper.
