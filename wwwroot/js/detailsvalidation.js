import * as base from './basevalidation.js';

form.addEventListener('submit', e =>
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
		if (!base.isPhoneValid(phoneValue))
		{
			phoneMessage += 'telefónne číslo má nesprávny tvar (+xxxxxxxxxxxx)';
		}
	}

	if (passwordValue !== '')
	{
		if (base.injectionProtection(passwordValue)) {
			passwordMessage += 'zadané heslo obashuje nepovolené kľúčové slová, ';
		}

		if (base.isPasswordValid(passwordValue)) {
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

	base.unsetFor('name');
	base.unsetFor('surname');
	base.unsetFor('phone');
	base.unsetFor('password');
	base.unsetFor('passwordConfirmation');

	if (nameMessage === '')
	{
		base.setSuccessFor('name');
	}
	else
	{
		base.setErrorFor('name', nameMessage);
	}

	if (surnameMessage === '')
	{
		base.setSuccessFor('surname');
	}
	else
	{
		base.setErrorFor('surname', surnameMessage);
	}

	if (phoneMessage === '')
	{
		base.setSuccessFor('phone');
	}
	else
	{
		base.setErrorFor('phone', phoneMessage);
	}

	if (passwordMessage === '')
	{
		base.setSuccessFor('password');
	}
	else
	{
		base.setErrorFor('password', passwordMessage.substring(0, passwordMessage.length - 2));
	}

	if (passwordConfirmationMessage === '')
	{
		base.setSuccessFor('passwordConfirmation');
	}
	else
	{
		base.setErrorFor('passwordConfirmation', passwordConfirmationMessage);
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