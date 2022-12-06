using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    /*  Pulling everything for this script from the Boss Design document (On Google Docs under GameDevChallenge folder)
     *  Hoping this all works out well.  Beginning this with pseudo code and comment heavy implementation, then 
     *  move on to actual code!
     *  TODO:
     *      1) create all local variables (private and public)
     *      2) create methods for the following: movement, spray attack, bomb attack, momentum attack, and enrage
     *      3) create health and methods to manipulate health
     *      4) ways to change sprite based on the action being taken
     *      5) need a game object for bombs (needs to be made completely)
     *      6) going to insulate all projectiles and attacks here where possible
     *          make a separate script for them if required (likely for attack projectiles)
     */

    //Baseline variables:
    //movement
    public float speed; //movement speed
    public float distance; //distance buffer between boss and walls
    public float timeLeftLower; //time between each change in movement direction (lower range)
    public float timeLeftUpper; //time between each change in movement direction (upper range)
    private float timeLeft;
                //NOTE: if distance is reached, timeLeft auto sets to 0 until direction corrects it
    public Rigidbody2D rb; //rigidbody for our boss
    private Vector3 movement;
    private Vector3 newMovement;
    private GameObject wall; //to avoid wall collision
    //health and target variables
    [SerializeField] private int health; //current health value
    public int maxHealth; //max health
    public int damage; //damage for all attacks
    private Transform target; //player
    public GameObject playerProjectile; //need this to calc health
    public float damageInterval;
    [SerializeField] private float damageCounter = 0;

    //Switches for various game states
    private bool attackActive; //determines visual state changes in Update
    private bool isExhausted;
    public float exhaustIntervalLower; //this is time between exhuast states, lower end
    public float exhaustIntervalUpper; //this is time between exhaust state, lower end
    public float exhaustIntervalChosen; //the chosen interval, changes after each execution
                        //NOTE: this needs to be a random range, exhaustIntervalChosen is the current value
    public float curExhaustInterval; //this changes in Update until it hits exhaustIntervalChosen
    private bool isEnraged;
    private bool isMoving; //prevent Update from constantly applying force to move

    //Attack objects
    public GameObject sprayProj;
    public GameObject momBlastProj;
    public Sprite[] momBlastTele = new Sprite[2]; //telegraph visuals
    public GameObject bombProj;

    /*  Attack variables
     *  firingPoints array:
     *      [0] north
     *      [1] northeast
     *      [2] east
     *      [3] southeast
     *      [4] south
     *      [5] southwest
     *      [6] west
     *      [7] northwest
     *  NOTE: May move all damage values to their respective objects
     */
    public GameObject[] firingPoints = new GameObject[8];
    //spray attack
    public float spraySpeed; //spray attack missile speed
    public float sprayCooldown; //cooldown between spray bursts
    public float sprayAttackSpeed; //cooldown between each attack wave in a burst
    private float sprayCounter; //counter for each burst of shots
    public float sprayCounterMax; //max amount of shots to be fired
    public float sprayTelegraph; //telegraph time (total)
    //momentum blast
    public float momBlastSpeed; //momentum blast missile speed
    public float momBlastCooldown; //cooldown between each round of momentum blasts
    public float momBlastTelegraph; //telegraph time (total)
    //bomb
    public float bombCooldown; //time between bomb cycles
    public float bombTime; //time between each bomb drop within the cycle
    private int bombNum; //number of spawned bombs
    public float maxBombs; //maximum number of spawned bombs


    /*  Game state visuals:
     *  5 game states (see comment at "visualState").
     *  States are the following in the array:
     *      [0] normal
     *      [1] telegraph spray attack
     *      [2] enraged
     *      [3] exhausted1
     *      [4] exhausted2
     */
    public SpriteRenderer render;
    public Sprite[] visualState = new Sprite[5]; // 5 game states: normal, exhausted (takes two), enraged, telegraph
    //public Sprite[] defaultVisual;

    // Start is called before the first frame update
    void Start()
    {
        //set max health
        health = maxHealth;
        //set visuals
        //set all booleans to false
        //set target to player
        target = FindObjectOfType<PlayerControl>().transform;

        //set wall to walls
        //set visualState[0] in renderer
        //set timeLeft
        timeLeft = Random.Range(timeLeftLower, timeLeftUpper);
        StartCoroutine(Move());

        //test lines below
        StartCoroutine(SprayInterval());
        StartCoroutine(BlastInterval());
        bombNum = 0;
        StartCoroutine(BombCooldown());
        StartCoroutine(Exhausted());
    }

    // Update is called once per frame
    void Update()
    {
        if (isExhausted == false || isEnraged == true)
        {
            if (isMoving == true)
            {
                transform.position = Vector2.MoveTowards(transform.position, newMovement, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + movement, speed * Time.deltaTime);
            }
        }
        else 
        { 
            
        }

        if (isEnraged != true)
        {
            Enrage(GetHealth(), GetMaxHealth());
        }

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }

        if (damageCounter < damageInterval)
        {
            damageCounter += Time.deltaTime;
        }
    }

    //setting the default visual state based on what condition boss is in
    public Sprite SetDefaultState()
    {
        if (isEnraged == true)
        {
            return visualState[2];
        }
        else if (isExhausted == true)
        {
            return visualState[3];
        }
        else
        {
            return visualState[0];
        }
    }

    //spray interval
    private IEnumerator SprayInterval()
    {
        yield return new WaitForSeconds(sprayCooldown);
        StartCoroutine(SprayTelegraph());
    }

    //spray telegraph
    private IEnumerator SprayTelegraph()
    {
        if (isExhausted == false)
        {
            int loop;
            for (loop = 0; loop < 4; loop++)
            {
                int temp = loop % 2;
                if (temp == 0)
                {
                    render.sprite = visualState[1];
                }
                else
                {
                    render.sprite = SetDefaultState();
                }

                yield return new WaitForSeconds((sprayTelegraph / 4));
            }

            if (isEnraged == true)
            {
                render.sprite = visualState[2];
            }
            else
            {
                render.sprite = SetDefaultState();
            }
        }
        else
        {
            render.sprite = SetDefaultState();
        }
        StartCoroutine(ShootSpray());
    }

    //spray attack
    private IEnumerator ShootSpray()
    {
        if (isExhausted == false)
        {
            int i;
            //cardinal directions
            render.sprite = visualState[1];
            yield return new WaitForSeconds(sprayAttackSpeed);
            render.sprite = SetDefaultState();

            for (i = 0; i < firingPoints.Length; i += 2)
            {
                GameObject projectile = Instantiate(sprayProj, firingPoints[i].transform.position, firingPoints[i].transform.rotation);
                Rigidbody2D riB = projectile.GetComponent<Rigidbody2D>();
                riB.AddForce(firingPoints[i].transform.up * spraySpeed, ForceMode2D.Impulse);
            }

            //StartCoroutine(WaitForSeconds(sprayAttackSpeed));
            yield return new WaitForSeconds(sprayAttackSpeed);
            //diagonals
            for (i = 1; i < firingPoints.Length; i += 2)
            {
                GameObject projectile = Instantiate(sprayProj, firingPoints[i].transform.position, firingPoints[i].transform.rotation);
                Rigidbody2D riB = projectile.GetComponent<Rigidbody2D>();
                riB.AddForce(firingPoints[i].transform.up * spraySpeed, ForceMode2D.Impulse);
            }
        }
        else
        {
            render.sprite = SetDefaultState();
        }

        //counter
        sprayCounter++;
        if (sprayCounter < sprayCounterMax)
        {
            StartCoroutine(ShootSpray());
        }
        else
        {
            StartCoroutine(SprayInterval());
            sprayCounter = 0f;
        }
    }

    //momentum blast interval
    private IEnumerator BlastInterval()
    {
        yield return new WaitForSeconds(momBlastCooldown);
        StartCoroutine(BlastTelegraph());
    }

    //momentum blast telegraph
    private IEnumerator BlastTelegraph()
    {
        if (isExhausted == false)
        {
            int i; //first firing point
            int j; //second firing point
            int loop; //loop variable

            i = Random.Range(0, firingPoints.Length);

            j = Random.Range(0, firingPoints.Length);

            while (i == j)
            {
                j = Random.Range(0, firingPoints.Length - 1);
            }

            GameObject projectile = Instantiate(momBlastProj, firingPoints[i].transform.position, firingPoints[i].transform.rotation);
            GameObject projectile2 = Instantiate(momBlastProj, firingPoints[j].transform.position, firingPoints[j].transform.rotation);
            projectile.GetComponent<BlastBounce>().isTelegraphing = true;
            projectile2.GetComponent<BlastBounce>().isTelegraphing = true;

            for (loop = 0; loop < 4; loop++)
            {
                int temp = loop % 2;
                projectile.gameObject.GetComponent<SpriteRenderer>().sprite = momBlastTele[temp];
                projectile2.gameObject.GetComponent<SpriteRenderer>().sprite = momBlastTele[temp];
                projectile.transform.position = firingPoints[i].transform.position;
                projectile2.transform.position = firingPoints[j].transform.position;
                yield return new WaitForSeconds(momBlastTelegraph / 4);
            }
            ShootBlast(i, j);
            Destroy(projectile);
            Destroy(projectile2);
        }
        else
        {
            StartCoroutine(BlastInterval());
        }
    }

    //momentum blast attack

    private void ShootBlast(int i, int j)
    {
        if (isExhausted == false)
        {
            //i's point
            GameObject projectile = Instantiate(momBlastProj, firingPoints[i].transform.position, firingPoints[i].transform.rotation);
            Rigidbody2D riB = projectile.GetComponent<Rigidbody2D>();
            riB.AddForce(firingPoints[i].transform.up * momBlastSpeed, ForceMode2D.Impulse);

            //j's point
            GameObject projectile2 = Instantiate(momBlastProj, firingPoints[j].transform.position, firingPoints[j].transform.rotation);
            Rigidbody2D riB2 = projectile2.GetComponent<Rigidbody2D>();
            riB2.AddForce(firingPoints[j].transform.up * momBlastSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(BlastInterval());
    }
    //bomb interval

    private IEnumerator BombCooldown()
    {
        yield return new WaitForSeconds(bombCooldown);
        StartCoroutine(BombCycle());
    }

    //bomb attack
    private IEnumerator BombCycle()
    {
        int i; //firing point
        i = Random.Range(0, firingPoints.Length - 1);

        if (bombNum < maxBombs)
        {
            bombNum++;
            Instantiate(bombProj, firingPoints[i].transform.position, transform.rotation);
            yield return new WaitForSeconds(bombTime);
            StartCoroutine(BombCycle());
        }
        else
        {
            bombNum = 0;
            StartCoroutine(BombCooldown());
        }
    }

    //exhausted interval
    private IEnumerator Exhausted()
    {
        if (isEnraged == true)
        {

        }
        else
        {
            //set the exhaust time at random and wait for interval
            exhaustIntervalChosen = Random.Range(exhaustIntervalLower, exhaustIntervalUpper);
            yield return new WaitForSeconds(exhaustIntervalChosen);

            //trigger exhaust window (short) and wait
            isExhausted = true;
            render.sprite = SetDefaultState();
            yield return new WaitForSeconds(5);

            //end exhaust period and reset timer to a new interval
            isExhausted = false;
            render.sprite = SetDefaultState();
            StartCoroutine(Exhausted());
        }
    }

    //enrage begin (this is only required to initiate the enrage boolean)
    private void Enrage(int current, int max)
    {
        if (((float)current / (float)max) * 100 < 30)
        {
            isEnraged = true;
            render.sprite = SetDefaultState();

            //set all timers and intervals to half
            //spray attack
            sprayCooldown /= 2;
            sprayAttackSpeed /= 2;
            sprayTelegraph /= 2;

        }
    }

    public bool GetDamagedState()
    {
        //just need this for health updates to properly register
        if (isExhausted == true || isEnraged == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //movement direction
    IEnumerator Move()
    {
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        timeLeft = Random.Range(timeLeftLower, timeLeftUpper);
        isMoving = false;
        yield return new WaitForSeconds(timeLeft);
        StartCoroutine(Move());
    }
    //if near wall, change direction
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            newMovement = new Vector2(target.position.x, target.position.y);
            isMoving = true;
        }
    }

    //THE FOLLOWING SECTION BELOW APPLIES STRICTLY TO BOSS HEALTH

    //trigger the health stuff on collision specifically with player missiles

    public void UpdateHealth(int mod)
    {
       if (damageCounter >= damageInterval)
        {
            health += mod;
            StartCoroutine(ShowDamage());
            damageCounter = 0f;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            health = 0;
        }
    }

    private IEnumerator ShowDamage()
    {
        render.sprite = visualState[4];
        yield return new WaitForSeconds(0.2f);
        render.sprite = SetDefaultState();
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
