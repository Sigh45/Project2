using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] float levelLoadDelay = 2f;
    
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dead;
    [SerializeField] AudioClip finish;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deadParticles;
    [SerializeField] ParticleSystem finishParticles;
    
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update(){
        if (state == State.Alive){
            // only rotate and RespondToThrustInput when alive
            RespondToRotateInput();
            RespondToThrustInput();
        }
    }

    private void RespondToRotateInput(){
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        // freeze rotation to take manual control
        rigidBody.freezeRotation = true;
        // rotate the ship left or right
        if(Input.GetKey(KeyCode.A)){
            // rotating left about the z-axis
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        
        else if(Input.GetKey(KeyCode.D)){
            // rotating right about the z-axis
             transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        // resume physics control  
        rigidBody.freezeRotation = false;
    }

    void OnCollisionEnter(Collision collision){
        if (state != State.Alive){
            return;
        }
        switch (collision.gameObject.tag) {
            case "Friendly":
               break;
               
            case "Finish":
               StartTransitionsSquences();
               break;

            default:
                StartDyingSquences();
                break; 
        }
    }

    private void StartTransitionsSquences(){
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finish);
        finishParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
     }

    private void StartDyingSquences(){
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(dead);
        deadParticles.Play();
        Invoke("LoadCurrentScene", levelLoadDelay);
    }

    private void RespondToThrustInput(){
        if (Input.GetKey(KeyCode.Space)){
           ApplyThrust();
        }
        else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
        }

    private void LoadNextScene(){
        // todo: allow for more then 2 level 
        SceneManager.LoadScene(1);
    }

    private void LoadCurrentScene(){ 
        SceneManager.LoadScene(0);
    }

    private void ApplyThrust(){
        // adds force to make rocket go up
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        // so audio doesnt play
        if(!audioSource.isPlaying){
            audioSource.PlayOneShot(mainEngine);
        } 
        mainEngineParticles.Play();
    }
} 