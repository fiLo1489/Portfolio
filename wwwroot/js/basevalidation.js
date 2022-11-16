function setErrorFor(element, message)
{
	const parent = document.getElementById(element);
	const children = document.getElementById(element + 'Message');

	parent.classList.add('error');
	children.textContent = message;
	children.style.visibility = 'visible';
}

function setSuccessFor(element)
{
	const parent = document.getElementById(element);
	parent.classList.add('success');
}

function unsetFor(element)
{
	const parent = document.getElementById(element);
	const children = document.getElementById(element + 'Message');

	if (parent.classList.contains('error'))
	{
		parent.classList.remove('error');
		children.textContent = '';
		children.style.visibility = 'hidden';
	}

	if (parent.classList.contains('success'))
	{
		parent.classList.remove('success');
		children.textContent = '';
		children.style.visibility = 'hidden';
	}
}

function areNumbers(value)
{
	return (/\d/.test(value));
}

function containsNumbers(value)
{
	return (/\d/.test(value));
}

function hasLowerCase(value)
{
	return (/[a-z]/.test(value));
}
function hasUpperCase(value)
{
	return (/[A-Z]/.test(value));
}

function injectionProtection(value)
{
	if (value.includes('insert') || value.includes('select') || value.includes('update') || value.includes('delete'))
	{
		return true;
	}
	else
	{
		return false;
	}
}

function isPasswordValid(value)
{
	if ((value.includes('!') || value.includes('.') || value.includes('&') || value.includes('#') || value.includes('/')) &&
		containsNumbers(value) && hasLowerCase(value) && hasUpperCase(value))
	{
		return true;
	}
	else
	{
		return false;
	}
}

function isPhoneValid(value)
{
	if (value.startsWith("+") &&
		value.length < 11 && value.length > 14 &&
		areNumbers(value.substring(1, value.length)))
	{
		return true;
	}
	else
	{
		return false;
	}
}

function isMailValid(value)
{
	if (value.includes('@') && value.includes('.') && value.length <= 255)
	{
		return true;
	}
	else
	{
		return false;
	}
}

export { setErrorFor, setSuccessFor, unsetFor, injectionProtection, isPasswordValid, isPhoneValid, isMailValid };