using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interactable is an interface with an OnInteract() function that is called when it's clicked on
public interface Interactable {

    public abstract void Interact(PlayerManager src);

}
