using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UV
{ 
    public class BackgroundController : MonoBehaviour
    {
        public void SetBackgroundPicture(Texture2D picture, Vector2 size)
        {
            var backgroundPicture = Instantiate(picture);
        }    
    }
}
