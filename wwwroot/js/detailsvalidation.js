﻿form.addEventListener('submit', e =>
{
	const nameValue = document.getElementById('nameInput').value.trim();
	const surnameValue = document.getElementById('surnameInput').value.trim();
	const phoneValue = document.getElementById('phoneInput').value.trim();
	const passwordValue = document.getElementById('passwordInput').value.trim();
	const passwordConfirmationValue = document.getElementById('passwordConfirmationInput').value.trim();

	var nameMessage = '';
	var surnameMessage = '';
	var phoneMessage = '';
	var passwordMessage = '';
	var passwordConfirmationMessage = '';

	if (nameValue === '')
	{
		nameMessage += 'nebolo zadané meno';
	}

	if (surnameValue === '')
	{
		surnameMessage += 'nebolo zadané priezvisko';
	}

	if (phoneValue === '')
	{
		phoneMessage += 'nebolo zadané telefónne číslo';
	}
	else
	{
		if (!phoneValue.startsWith("+") ||
			phoneValue.length < 11 || phoneValue.length > 14 ||
			!areNumbers(phoneValue.substring(1, phoneValue.length)))
		{
			phoneMessage += 'telefónne číslo má nesprávny tvar (+xxxxxxxxxxxx)';
		}
	}

	if (passwordValue !== '')
	{
		if (injectionProtection(passwordValue)) {
			passwordMessage += 'zadané heslo obashuje nepovolené kľúčové slová, ';
		}

		if (!(passwordValue.includes('!') || passwordValue.includes('.') || passwordValue.includes('&') || passwordValue.includes('#') || passwordValue.includes('/')) ||
			!(containsNumbers(passwordValue) &&
				hasLowerCase(passwordValue) &&
				hasUpperCase(passwordValue))) {
			passwordMessage += 'heslo musí obsahovať veľký a malý znak, číslo a bezpečnostný znak (! . & # /), ';
		}
	}

	if (passwordConfirmationValue === '' && passwordValue !== '')
	{
		passwordConfirmationMessage += 'nebolo zadané potvrdenie hesla';
	}
	else
	{
		if (passwordValue !== passwordConfirmationValue)
		{
			passwordConfirmationMessage += 'heslá sa nezhodujú';
		}
	}

	unsetFor('name');
	unsetFor('surname');
	unsetFor('phone');
	unsetFor('password');
	unsetFor('passwordConfirmation');

	if (nameMessage === '')
	{
		setSuccessFor('name');
	}
	else
	{
		setErrorFor('name', nameMessage);
	}

	if (surnameMessage === '')
	{
		setSuccessFor('surname');
	}
	else
	{
		setErrorFor('surname', surnameMessage);
	}

	if (phoneMessage === '')
	{
		setSuccessFor('phone');
	}
	else
	{
		setErrorFor('phone', phoneMessage);
	}

	if (passwordMessage === '')
	{
		setSuccessFor('password');
	}
	else
	{
		setErrorFor('password', passwordMessage.substring(0, passwordMessage.length - 2));
	}

	if (passwordConfirmationMessage === '')
	{
		setSuccessFor('passwordConfirmation');
	}
	else
	{
		setErrorFor('passwordConfirmation', passwordConfirmationMessage);
	}

	var submit;

	if (nameMessage === '' && surnameMessage === '' && phoneMessage === '' && passwordMessage === '' && passwordConfirmationMessage === '')
	{
		submit = true;
	}
	else {
		submit = false;
	}

	if (!submit) {
		e.preventDefault();
	}
});

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