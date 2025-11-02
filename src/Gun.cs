using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    // --- INFO DEL ARMA ---
    public float damage = 10f;        // cuanto pega el tiro
    public float rango = 100f;        // hasta donde llega el tiro
    public float ratiodisparo = 10f;  // tiros por seg
    public Camera fpsCam;             // la camara desde donde se dispara

    // --- MUNICION ---
    public int maxAmmo = 30;          // balas en cargador
    public int reserveAmmo = 90;      // balas en reserva
    public int currentAmmo;           // balas actuales
    public float reloadTime = 1.5f;   // tiempo para recargar
    private bool isReloading = false; // si esta recargando
    public bool dobletiro = false;    // si puede disparar doble
    public bool sinMunicion = false;  // si no queda muni

    // --- EFECTOS ---
    public ParticleSystem muzzleFlash;  // efecto al disparar
    public GameObject impactEffect;     // efecto al pegar
    public GameObject explosionEffect;  // efecto si es bala explosiva

    // --- RECOIL ---
    public Transform[] recoilObjects;    // objetos que se mueven al disparar
    public Vector3 recoilAmount = new Vector3(0,0,-0.2f); // cuanto se mueve
    public float recoilSpeed = 10f;      // velocidad para volver a la pos original
    private Vector3[] originalPositions; // guardo las posiciones iniciales
    private float nextTimeToFire = 0f;   // para controlar cadencia
    public PlayerVida player;            // referencia al player para puntaje

    // --- RECARGA VISUAL ---
    public Transform pistolTransform;
    public Transform magazineTransform;
    public Vector3 pistolOffset = new Vector3(0,0.2f,0);   // cuanto baja el arma
    public Vector3 magazineOffset = new Vector3(0,-0.2f,0);// cuanto baja el cargador
    private Vector3 pistolOriginalPos;
    private Vector3 magazineOriginalPos;

    // --- MODO EXPLOSIVO ---
    private bool explosivo = false;      // si esta activado
    public float radioExplosion = 5f;    // radio de la explosion
    public float fuerzaExplosion = 500f; // fuerza de la explosion

    void Start()
    {
        currentAmmo = maxAmmo; // cargador lleno
        pistolOriginalPos = pistolTransform.localPosition; 
        magazineOriginalPos = magazineTransform.localPosition;

        // guardo posiciones originales del recoil
        if(recoilObjects != null)
        {
            originalPositions = new Vector3[recoilObjects.Length]; // creo array del mismo tamaño
            // recorro cada objeto de recoil
            for(int i=0; i<recoilObjects.Length; i++)
            {
                if(recoilObjects[i]!=null)
                    originalPositions[i] = recoilObjects[i].localPosition; // guardo pos original
            }
        }

        ActualizarEstadoMunicion(); // chequeo si hay muni
    }

    void OnEnable()
    {
        isReloading = false; // cuando activamos arma no recarga
        ActualizarEstadoMunicion(); // chequeo muni
    }

    public void addmuni(int a)
    {
        reserveAmmo += a; // sumo balas a reserva
    }

    void Update()
    {
        if(isReloading) return; // no hacer nada si recarga

        // RECARGA
        if(Input.GetKeyDown(KeyCode.R) && currentAmmo<maxAmmo && reserveAmmo>0)
        {
            StartCoroutine(Reload());
            return;
        }

        // DISPARO
        if(Input.GetButton("Fire1") && Time.time>=nextTimeToFire)
        {
            if(currentAmmo<=0)
            {
                Debug.Log("SIN MUNI, presiona R");
                ActualizarEstadoMunicion();
                return;
            }

            nextTimeToFire = Time.time + 1f/ratiodisparo; // siguiente disparo
            Shoot();
        }

        // RECOIL SUAVE
        if(recoilObjects != null)
        {
            for(int i=0; i<recoilObjects.Length; i++) // recorro objetos de recoil
            {
                if(recoilObjects[i]!=null)
                    // hago que vuelva despacio a su pos original
                    recoilObjects[i].localPosition = Vector3.Lerp(
                        recoilObjects[i].localPosition,
                        originalPositions[i],
                        recoilSpeed*Time.deltaTime
                    );
            }
        }
    }

    public void PonerDaño(float newDamage)
    {
        damage = newDamage; // cambio daño
    }

    public float GetDamage()
    {
        return damage; // retorno daño
    }

    public void ActivarExplosivo(bool estado)
    {
        explosivo = estado; // activo/desactivo modo explosivo
    }

    void Shoot()
    {
        currentAmmo--; // saco una bala
        ActualizarEstadoMunicion();
        muzzleFlash?.Play(); // efecto de disparo

        // RECOIL RAPIDO
        if(recoilObjects != null)
            foreach(var obj in recoilObjects)
                if(obj!=null) obj.localPosition += recoilAmount;

        // creo ray para ver si pego
        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction*rango, Color.red, 1f);

        if(Physics.Raycast(ray, out RaycastHit hit, rango))
        {
            // chequeo si pego enemigo
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if(enemy)
            {
                enemy.TakeDamage(damage); // hago daño
                player?.AddScore(50);     // sumo puntaje
            }

            // efecto normal de impacto
            if(impactEffect)
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }

            // efecto explosivo
            if(explosivo)
            {
                if(explosionEffect)
                    Destroy(Instantiate(explosionEffect, hit.point, Quaternion.identity), 0.2f);

                Collider[] objs = Physics.OverlapSphere(hit.point, radioExplosion); // todos los objetos cerca
                foreach(var obj in objs) // recorro todos
                {
                    EnemyHealth e = obj.GetComponentInParent<EnemyHealth>();
                    if(e) e.TakeDamage(damage/2f); // daño a cercanos

                    Rigidbody rb = obj.attachedRigidbody;
                    if(rb!=null) rb.AddExplosionForce(fuerzaExplosion, hit.point, radioExplosion);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return MoveGunForReload(reloadTime); // animacion recarga

        int neededAmmo = maxAmmo - currentAmmo; 
        int ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        pistolTransform.localPosition = pistolOriginalPos + pistolOffset;
        magazineTransform.localPosition = magazineOriginalPos + magazineOffset;

        isReloading = false;
        ActualizarEstadoMunicion();
    }

    public bool GetDobleTiro() { return dobletiro; }
    public void SetDobleTiro(bool valor) { dobletiro = valor; }

    void ActualizarEstadoMunicion()
    {
        sinMunicion = (currentAmmo<=0 && reserveAmmo<=0); // chequeo si ya no hay muni
    }

    IEnumerator MoveGunForReload(float duration)
    {
        float t = 0f;
        float half = duration*0.5f;

        // bajar arma
        while(t<half)
        {
            t += Time.deltaTime;
            float p = t/half;
            pistolTransform.localPosition = Vector3.Lerp(pistolOriginalPos, pistolOriginalPos+pistolOffset, p);
            magazineTransform.localPosition = Vector3.Lerp(magazineOriginalPos, magazineOriginalPos+magazineOffset, p);
            yield return null;
        }

        // subir arma
        t=0f;
        while(t<half)
        {
            t += Time.deltaTime;
            float p = t/half;
            pistolTransform.localPosition = Vector3.Lerp(pistolOriginalPos+pistolOffset, pistolOriginalPos, p);
            magazineTransform.localPosition = Vector3.Lerp(magazineOriginalPos+magazineOffset, magazineOriginalPos, p);
            yield return null;
        }
    }
}
