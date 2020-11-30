using UnityEngine;

namespace ArtemisProjectile.Demo
{
    public class ExampleImplementation : ProjectileController
    {
        public GameObject bulletmarkPrefab;
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
            Destroy(gameObject);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
