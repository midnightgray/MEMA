using UnityEngine;
using UnityEngine.UI;

public class ProfilePicture : MonoBehaviour
{
    public RawImage selectedImage;
    public RawImage updatingImage;
    public GameObject panel;

    void Start()
    {
        if (PlayerPrefs.HasKey("pfp_path"))
        {
            string path = PlayerPrefs.GetString("pfp_path", "None");
            SavedPfP(path, 512);
        }
    }

    public void HidePanel()
    {
        panel.gameObject.SetActive (false);
    }

    public void imageSelect(){
        PickImage(512);
    }

    public void SavingImage(){
        selectedImage.texture = updatingImage.texture;
        HidePanel();
    }
    public void deleteprofile()
    {
        PlayerPrefs.DeleteKey("pfp_path");
        selectedImage.texture = null;
        HidePanel();
    }
    private void SavedPfP(string path, int maxSize){
        if( path != null )
        {
            // Create Texture from selected image
            Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
            if( texture == null )
            {
                Debug.Log( "Couldn't load texture from " + path );
                return;
            }
            selectedImage.texture = texture;
        }
    }

    private void PickImage( int maxSize )
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            PlayerPrefs.SetString("pfp_path", path);
            PlayerPrefs.Save();
            Debug.Log( "Image path: " + path );
            LoadPfP(path, maxSize);
        } );

        Debug.Log( "Permission result: " + permission );
    }

    private void LoadPfP(string path, int maxSize){
        if( path != null )
        {
            // Create Texture from selected image
            Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
            if( texture == null )
            {
                Debug.Log( "Couldn't load texture from " + path );
                return;
            }
            updatingImage.texture = texture;
        }
    }
}
