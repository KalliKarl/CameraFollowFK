using UnityEngine;

public class CamFollow : MonoBehaviour
{

	public Transform target;
	private Vector3 targetPosition;
	private Vector3 desiredPosition;
	private float x, y, z;
	public bool followTargetX, followTargetY, followTargetZ;
	[SerializeField] private float _cameraFollowSpeed;

	public bool customOffset;
	public Vector3 customOffsetPos;
	private Vector3 _startOfffset;

	[Header("Update")]
	public bool update;
	public bool fixedUpdate;
	public bool lateUpdate;

	[Header("Zoom")]
	[SerializeField] private bool _zoomActive;
	[SerializeField] private float _zoomMinimum;
	[SerializeField] private float _zoomMaximum;
	[SerializeField] private float _zoomSensitivity;
	private Vector2 _mousePosition;
	private float _zoom;

	private bool rightClickHold;
	private bool leftClickHold;
	[Header("Drag Camera")]
	[SerializeField] private bool _moveXActive;
	[Range(0.0f, 3f)]
	[SerializeField] private float moveXSensitivity;

	[SerializeField] private bool _moveYActive;
	[Range(0.0f, 3f)]
	[SerializeField] private float moveYSensitivity;
	private float viewPortWidth;
	private float viewPortHeight;
	private float ratioH;
	private float ratioW;
	[SerializeField] private float increment;
	private float incrementY;
	private float incrementX;
	private Camera _mainCamera;
	[Range(0.0f, 0.5f)]
	[SerializeField] private float yBorderThickness;
	[Range(0.0f, 0.5f)]
	[SerializeField] private float xBorderThinckness;
	private float aspectRatio;
	private bool isPortrait;

	private void Awake()
	{
		_startOfffset = transform.position - target.position;
		_mainCamera = Camera.main;
		viewPortWidth = _mainCamera.pixelRect.width;
		viewPortHeight = _mainCamera.pixelRect.height;

	}
	private void Start()
	{
		targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
		desiredPosition = targetPosition + (customOffset ? customOffsetPos : _startOfffset);
		transform.position = desiredPosition;
	}

	void Update()
	{
		rightClickHold = (Input.GetMouseButton(1));
		leftClickHold = (Input.GetMouseButton(0));
		if (target != null)
		{
			_mousePosition = _mainCamera.ScreenToViewportPoint(Input.mousePosition);

			targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
			desiredPosition = targetPosition + (customOffset ? customOffsetPos : _startOfffset);
			x = desiredPosition.x * .5f;
			y = desiredPosition.y;
			z = desiredPosition.z;

			if (rightClickHold)
			{
				viewPortHeight = _mainCamera.pixelRect.height;
				viewPortWidth = _mainCamera.pixelRect.width;


				if (viewPortHeight > viewPortWidth)
				{
					aspectRatio = (viewPortHeight / viewPortWidth);
					isPortrait = true;
				}
				else if (viewPortWidth > viewPortHeight)
				{
					aspectRatio = (viewPortWidth / viewPortHeight);
					isPortrait = false;
				}

				ratioH = (viewPortHeight / viewPortWidth);
				ratioW = (viewPortWidth / viewPortHeight);

				incrementX = (increment * ((isPortrait) ? aspectRatio : 1) * moveXSensitivity);
				incrementY = (increment * ((!isPortrait) ? aspectRatio : 1) * moveYSensitivity);
			}

			if (_zoomActive)
				Zoom();

			if (_moveXActive && rightClickHold)
				MoveOffsetX();

			if (_moveYActive && rightClickHold)
				MoveOffestY();

			if (update)
				PositionLerp();
		}

	}

	private void LateUpdate()
	{
		if (target != null && lateUpdate)
			PositionLerp();
	}
	private void FixedUpdate()
	{
		if (target != null && fixedUpdate)
			PositionLerp();
	}

	private void PositionLerp()
	{
		transform.position = Vector3.Lerp(transform.position,
					  new Vector3(followTargetX ? x : transform.position.x, followTargetY ? y : transform.position.y, followTargetZ ? z : transform.position.z), Time.deltaTime * _cameraFollowSpeed);
	}
	public void StopFollow()
	{
		followTargetX = false;
		followTargetY = false;
		followTargetZ = false;

	}

	private void Zoom()
	{
		_zoom = Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
		if (_zoom < 0 && (customOffset ? customOffsetPos.y : _startOfffset.y) > _zoomMinimum)
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x, ((customOffsetPos.y + _zoom) < 0) ? 0 : customOffsetPos.y + _zoom, customOffsetPos.z);
			else
				_startOfffset = new Vector3(_startOfffset.x, ((_startOfffset.y + _zoom) < 0) ? 0 : _startOfffset.y + _zoom, _startOfffset.z);
		}
		else if (_zoom > 0 && (customOffset ? customOffsetPos.y : _startOfffset.y) < _zoomMaximum)
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x, ((customOffsetPos.y + _zoom) < 0) ? 0 : customOffsetPos.y + _zoom, customOffsetPos.z);
			else
				_startOfffset = new Vector3(_startOfffset.x, ((_startOfffset.y + _zoom) < 0) ? 0 : _startOfffset.y + _zoom, _startOfffset.z);
		}
	}

	private void MoveOffestY()
	{

		if (_mousePosition.y < yBorderThickness)
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x, customOffsetPos.y, customOffsetPos.z - incrementY);
			else
				_startOfffset = new Vector3(_startOfffset.x, _startOfffset.y, _startOfffset.z - incrementY);
		}
		else if (_mousePosition.y > (1 - yBorderThickness))
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x, customOffsetPos.y, customOffsetPos.z + incrementY);
			else
				_startOfffset = new Vector3(_startOfffset.x, _startOfffset.y, _startOfffset.z + incrementY);
		}
	}

	private void MoveOffsetX()
	{




		if (_mousePosition.x < xBorderThinckness)
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x - incrementX, customOffsetPos.y, customOffsetPos.z);
			else
				_startOfffset = new Vector3(_startOfffset.x - incrementX, _startOfffset.y, _startOfffset.z);
		}
		else if (_mousePosition.x > (1 - xBorderThinckness))
		{
			if (customOffset)
				customOffsetPos = new Vector3(customOffsetPos.x + incrementX, customOffsetPos.y, customOffsetPos.z);
			else
				_startOfffset = new Vector3(_startOfffset.x + incrementX, _startOfffset.y, _startOfffset.z);
		}
	}

}
