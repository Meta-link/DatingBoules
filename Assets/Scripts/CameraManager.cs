using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float maxZoom = 2f;
    public float zoomSpeed = 1f;
    public float moveSpeed = 1f;
    public float distanceMax = 1f;
    public float resetZoomSpeed = 1f;
    public float resetPositionSpeed = 1f;

    private Camera cam;
    private float _startZoom;
    private Vector3 _startPosition;
    private GameObject _startboule;
    private ShakeCamera _shake;

    void Start () {
        cam = GetComponent<Camera>();
        _startZoom = cam.orthographicSize;
        _startPosition = transform.position;
        _startboule = GameObject.Find("SpriteStart");
        _shake = GetComponent<ShakeCamera>();

        //EVENTS
        GameManager.OnState += _OnState;
        GameManager.OnShake += _OnShake;
	}

    void OnDestroy()
    {
        GameManager.OnState -= _OnState;
        GameManager.OnShake += _OnShake;
    }
	
	void Update () {
		switch(GameManager.state)
        {
            case EState.CHOOSE:
                {
                    if(cam.orthographicSize > maxZoom)
                    {
                        cam.orthographicSize -= zoomSpeed * Time.deltaTime;
                        if(cam.orthographicSize <= maxZoom)
                        {
                            cam.orthographicSize = maxZoom;
                        }
                    }

                    if(Vector3.Distance(transform.position, _startboule.transform.position) >= distanceMax)
                    {
                        Vector3 tmp = Vector3.MoveTowards(transform.position, _startboule.transform.position, moveSpeed * Time.deltaTime);
                        float limiteX = _startboule.transform.parent.position.x - 1;
                        float limiteY = _startboule.transform.parent.position.y;
                        transform.position = new Vector3(transform.position.x < limiteX ? limiteX : tmp.x, transform.position.y > limiteY ? limiteY : tmp.y, _startPosition.z);
                    }
                    break;
                }
            default:
                {
                    if(cam.orthographicSize < _startZoom)
                    {
                        cam.orthographicSize += resetZoomSpeed * Time.deltaTime;
                        if (cam.orthographicSize >= _startZoom)
                        {
                            cam.orthographicSize = _startZoom;
                        }
                    }

                    if(transform.position != _startPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _startPosition, moveSpeed * Time.deltaTime);
                        if(Vector3.Distance(transform.position, _startPosition) <= 0.5f)
                        {
                            transform.position = _startPosition;
                        }
                    }

                    break;
                }
        }
	}

    private void _OnState(EState state)
    {
        switch(state)
        {
            case EState.DOWNING:
                {
                    cam.orthographicSize = _startZoom;
                    break;
                }
        }
    }

    private void _OnShake()
    {
        _shake.DoShake(0.01f);
    }
}
