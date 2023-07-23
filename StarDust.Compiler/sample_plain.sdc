fn void main()
{
	var first = 2;
	var second = 2;
	var result = sum(first, second);
	if(first > second)
		return;

	var a = 1;
}

fn int sum(int a, int b)
{
	var result = a + b;
	return result;
}

fn void PrintEven()
{
	var i = 1;
	while (i < 10)
	{
		if ((i / 2) * 2 == i)
		{
			//print(i);
			i = i + 1;
		}
	}
}

fn void PrintOdd()
{
	var i = 1;
	while (i < 10)
	{
		if (!((i / 2) * 2 == i))
		{
			//print(i);
			i = i + 1;
		}
	}
}