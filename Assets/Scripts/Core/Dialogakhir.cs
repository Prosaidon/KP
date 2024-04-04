using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogakhir : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    // Start is called before the first frame update
    private Player player; // Tambahkan referensi ke skrip Player

    void Start()
    {
        textComponent.text = string.Empty;
        player = FindObjectOfType<Player>(); // Cari dan simpan referensi ke Player
        StartDialog();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialog()
    {
        index = 0;
        StartCoroutine(TypeLine());
        player.SetCanMove(false);
    }

    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if(index < lines.Length -1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else{
            player.SetCanMove(true);
            gameObject.SetActive(false);
        }
    }
}
