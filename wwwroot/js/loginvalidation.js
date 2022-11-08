form.addEventListener('submit', e =>
{
	const mailValue = document.getElementById('mailInput').value.trim();
	const passwordValue = document.getElementById('passwordInput').value.trim();

	var mailMessage = '';
	var passwordMessage = '';

	if (mailValue === '')
	{
		mailMessage += 'nebol zadaný mail, ';
	}
	else {
		if (mailValue.includes('insert ') || mailValue.includes('select ') || mailValue.includes('update ') || mailValue.includes('delete '))
		{
			mailMessage += 'zadaný mail obashuje nepovolené kľúčové slová, ';
		}

		if (!mailValue.includes('@') || !mailValue.includes('.'))
		{
			mailMessage += 'zadaný mail má nesprávny tvar, ';
		}

		if (mailValue.length > 255)
		{
			mailMessage += 'zadaný mail je príliš dlhý, ';
		}
	}

	if (passwordValue === '')
	{
		passwordMessage += 'nebol zadaný mail, ';
	}
	else
	{
		if (passwordValue.includes('insert ') || passwordValue.includes('select ') || passwordValue.includes('update ') || mailValue.includes('delete ')) {
			passwordMessage += 'zadané heslo obashuje nepovolené kľúčové slová, ';
		}

		if (!(passwordValue.includes('!') || passwordValue.includes('.') || passwordValue.includes('&') || passwordValue.includes('#') || passwordValue.includes('/')) ||
			!(containsNumbers(passwordValue) &&
				hasLowerCase(passwordValue) &&
				hasUpperCase(passwordValue)))
		{
			passwordMessage += 'heslo musí obsahovať veľký a malý znak, číslo a bezpečnostný znak (! . & # /), ';
		}
	}

	unsetFor('mail');
	unsetFor('password');

	if (mailMessage === '')
	{
		setSuccessFor('mail');
	}
	else {
		setErrorFor('mail', mailMessage.substring(0, mailMessage.length - 2));
	}

	if (passwordMessage === '')
	{
		setSuccessFor('password');
	}
	else {
		setErrorFor('password', passwordMessage.substring(0, passwordMessage.length - 2));
	}

	var submit;

	if (passwordMessage === '' && mailMessage === '')
	{
		submit = true;
	}
	else
	{
		submit = false;
	}

	if (!submit)
	{
		e.preventDefault();
	}
});

function setErrorFor(element, message)
{
	const parent = document.getElementById(element);
	const children = document.getElementById(element + 'Message');

	parent.classList.add('error');
	children.textContent = message;
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
	}

	if (parent.classList.contains('success'))
	{
		parent.classList.remove('success');
		children.textContent = '';
	}
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