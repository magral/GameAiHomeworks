Leah Magrane - Assignment 2 : Multi agent movement

In order to change the algorithm running click one of the white butterflies and check either the cone collision or collision prediction box. Click apply to prefab so that all the butterflies 
follow the same algorithm, or you can have some follow one algorithm and the rest follow a different one. 

The path will loop indefinitely. You can check the orientation of the butterflies in the scene view by looking at the small white line. The long white line is the position they are tracking.

Collision prediction is working by taking the abs(distance(pt, pc)) and using that to avoid. 

white butterflies evading by pausing in place while waiting for the other group to pass

-- Leah