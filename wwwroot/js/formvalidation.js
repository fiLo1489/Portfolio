form.addEventListener('submit', e =>
{
	const noteValue = document.getElementById('noteInput').value.trim();
	const dateValue = Date.parse(document.getElementById('dateInput').value);
	const todayValue = Date.parse(new Date().toJSON().slice(0, 10).replace(/-/g, '-'));
	const gap = 32400000000;

	var dateMessage = '';
	var noteMessage = '';

	if (todayValue > dateValue)
	{
		dateMessage += 'dátum musí byť budúci';
	}
	else if ((todayValue + gap) < dateValue)
	{
		dateMessage += 'dátum musí byť v priebehu následujúceho roku';
	}

	if (noteValue !== '')
	{
		if (injectionProtection(noteValue))
		{
			noteMessage += 'poznámka obsahuje nepovolené kľúčové slová';
		}
	}

	unsetFor('note');
	unsetFor('date');

	if (noteMessage === '')
	{
		setSuccessFor('note');
	}
	else
	{
		setErrorFor('note', noteMessage);
	}

	if (dateMessage === '')
	{
		setSuccessFor('date');
	}
	else
	{
		setErrorFor('date', dateMessage);
	}

	var submit;

	if (noteMessage === '' && dateMessage === '') {
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

function injectionProtection(value)
{
	if (value.includes('insert ') || value.includes('select ') || value.includes('update ') || value.includes('delete '))
	{
		return true;
	}
	else
	{
		return false;
	}
}