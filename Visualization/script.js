var positionList = [{ 'x':0, 'y':0 },{ 'x':0, 'y':0 },{ 'x':1, 'y':0 },{ 'x':3, 'y':-1 },{ 'x':4, 'y':0 },{ 'x':3, 'y':0 },{ 'x':1, 'y':-1 },{ 'x':3, 'y':0 },{ 'x':7, 'y':2 },{ 'x':5, 'y':-1 },{ 'x':4, 'y':1 },{ 'x':2, 'y':4 },{ 'x':0, 'y':1 },{ 'x':3, 'y':0 },{ 'x':8, 'y':0 },{ 'x':12, 'y':-2 },{ 'x':14, 'y':-4 },{ 'x':11, 'y':-6 },{ 'x':12, 'y':-2 },{ 'x':14, 'y':-5 },{ 'x':16, 'y':-1 },{ 'x':12, 'y':-1 },{ 'x':16, 'y':2 },{ 'x':13, 'y':4 },{ 'x':16, 'y':6 },{ 'x':19, 'y':7 },{ 'x':17, 'y':11 },{ 'x':20, 'y':11 },{ 'x':18, 'y':15 },{ 'x':17, 'y':12 },{ 'x':21, 'y':12 },{ 'x':20, 'y':15 },{ 'x':17, 'y':13 },{ 'x':21, 'y':11 },{ 'x':21, 'y':15 },{ 'x':24, 'y':12 },{ 'x':26, 'y':9 },{ 'x':28, 'y':12 },{ 'x':30, 'y':17 },{ 'x':31, 'y':14 },{ 'x':34, 'y':16 },{ 'x':36, 'y':19 },{ 'x':32, 'y':20 },{ 'x':32, 'y':16 },{ 'x':33, 'y':12 },{ 'x':33, 'y':9 },{ 'x':36, 'y':11 },{ 'x':33, 'y':13 },{ 'x':30, 'y':16 },{ 'x':27, 'y':18 },{ 'x':31, 'y':19 },{ 'x':29, 'y':16 },{ 'x':27, 'y':13 },{ 'x':25, 'y':10 },{ 'x':22, 'y':13 },{ 'x':21, 'y':16 },{ 'x':18, 'y':13 },{ 'x':17, 'y':17 },{ 'x':16, 'y':20 },{ 'x':17, 'y':24 },{ 'x':19, 'y':22 },{ 'x':16, 'y':20 },{ 'x':18, 'y':18 },{ 'x':20, 'y':15 },{ 'x':22, 'y':18 },{ 'x':23, 'y':22 },{ 'x':19, 'y':22 },{ 'x':21, 'y':24 },{ 'x':18, 'y':25 },{ 'x':18, 'y':22 },{ 'x':20, 'y':24 },{ 'x':23, 'y':27 },{ 'x':26, 'y':29 },{ 'x':25, 'y':25 },{ 'x':29, 'y':25 },{ 'x':26, 'y':29 },{ 'x':25, 'y':25 },{ 'x':23, 'y':23 },{ 'x':26, 'y':22 },{ 'x':24, 'y':20 },{ 'x':27, 'y':18 },{ 'x':27, 'y':22 },{ 'x':24, 'y':21 },{ 'x':26, 'y':18 },{ 'x':23, 'y':18 },{ 'x':25, 'y':20 },{ 'x':27, 'y':22 },{ 'x':25, 'y':24 },{ 'x':24, 'y':22 },{ 'x':24, 'y':24 },{ 'x':24, 'y':22 },{ 'x':24, 'y':24 },{ 'x':22, 'y':24 },{ 'x':21, 'y':22 },{ 'x':22, 'y':24 },{ 'x':20, 'y':25 },{ 'x':19, 'y':24 },{ 'x':17, 'y':23 },{ 'x':15, 'y':24 },{ 'x':17, 'y':24 },{ 'x':18, 'y':25 },{ 'x':19, 'y':26 },{ 'x':21, 'y':26 },{ 'x':19, 'y':25 },{ 'x':21, 'y':26 },{ 'x':21, 'y':28 },{ 'x':22, 'y':29 },{ 'x':20, 'y':31 },{ 'x':23, 'y':30 },{ 'x':26, 'y':27 },{ 'x':23, 'y':27 },{ 'x':19, 'y':26 },{ 'x':20, 'y':23 },{ 'x':15, 'y':23 },{ 'x':18, 'y':26 },{ 'x':15, 'y':27 },{ 'x':11, 'y':29 },{ 'x':11, 'y':25 },{ 'x':9, 'y':27 },{ 'x':8, 'y':25 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 },{ 'x':8, 'y':26 }];


/*
var canvas = document.createElement('canvas');
canvas.id = "CursorLayer";
canvas.width = 1224;
canvas.height = 768;
canvas.style.zIndex = 8;
canvas.style.position = "absolute";
canvas.style.border = "1px solid";
var body = document.getElementsByTagName("body")[0];
body.appendChild(canvas);*/





function init()
{
	
	var canvas = document.getElementById("myCanvas");
	canvas.width = 1224;
	canvas.height = 768;
	canvas.style.zIndex = 8;
	canvas.style.position = "absolute";
	canvas.style.border = "1px solid";


	var canvasWidth = canvas.width;
	var canvasHeight = canvas.height;
	var ctx = canvas.getContext("2d");
	var canvasData = ctx.getImageData(0, 0, canvasWidth, canvasHeight);


	var scaleFactor = 5;

	var ctx = canvas.getContext("2d");

	x = 200;
	y = 200;

	var p 
	for(p= 0; p< positionList.length; p=p+1)
	{

	  ctx.fillRect(positionList[p].x*scaleFactor +x, positionList[p].y*scaleFactor+y,scaleFactor,scaleFactor)
	  console.log(positionList[p].x);
	}
}

//updateCanvas();






