using UnityEngine;

namespace ArtemisProjectile.Demo
{
    public class ExampleImplementation : ProjectileController
    {
        public GameObject bulletmarkPrefab;
        public GameObject sparksPrefab;
        private Color color;

        private void Start()
        {
            color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }
        protected override void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness)
        {
            var mark = Instantiate(bulletmarkPrefab, entry.point + entry.normal * 0.01f, Quaternion.LookRotation(entry.normal));
            mark.GetComponent<SpriteRenderer>().color = color;
        }

        protected override void OnPenetrationExit(RaycastHit exit, Vector3 velocity)
        {
            Instantiate(bulletmarkPrefab, exit.point + exit.normal * 0.01f, Quaternion.LookRotation(exit.normal)).GetComponent<SpriteRenderer>().color = color;
        }

        protected override void OnPenetrationFailed(RaycastHit hit, Vector3 velocity, float thickness)
        {
            //spawn sparks
            ParticleSystem sparks = Instantiate(
                          sparksPrefab,
                          hit.point + Vector3.Reflect(velocity.normalized, hit.normal) * 0.15f,
                          Quaternion.LookRotation(Vector3.Reflect(velocity.normalized, hit.normal))).GetComponent<ParticleSystem>();

            var hitAngle = Vector3.Angle(velocity.normalized, hit.normal);
            Debug.Log((180 - hitAngle) / 180);

            var main = sparks.main;
            main.startSpeed = velocity.magnitude / 25 * Mathf.Max((180 - hitAngle) / 180, 0.1f);

            var shape = sparks.shape;
            shape.angle = hitAngle - 90;

            sparks.Play();
            //Destroy sparks after 3 seconds
            Destroy(sparks.gameObject, 3);

            //Destroy Bullet
            Destroy(gameObject);
        }
    }
}
