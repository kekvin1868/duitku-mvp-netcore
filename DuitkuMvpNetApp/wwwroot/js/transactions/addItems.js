$(document).ready(function() {
    var currentNo = 1;
    var arrayItems = [];
    var transactionId = document.getElementById('transactionId').value;

    $('#addButton').click(function() {
        // Clear previous errors
        $('#namaError').text('');
        $('#hargaError').text('');

        // Get form data
        var nama = $('input[name="Nama"]').val();
        var harga = $('input[name="Harga"]').val();
        
        // Basic validation
        if (nama === '') {
            $('#namaError').text('Nama is required.');
            return;
        }
        var regex = /^[1-9]\d*$/;
        if (harga === '' || !regex.test(harga)) {
            $('#hargaError').text('Harga must be a positive number.');
            return;
        }
        var hargaValue = parseInt(harga);
        if (hargaValue < 10000) {
            $('#hargaError').text('Harga must be at least 10,000.');
            return;
        }
        
        // Create new row
        var newRow = '<tr><td>' + currentNo + '</td><td>' + nama + '</td><td>' + harga + '</td></tr>';
        console.log("New row added: ", newRow);
        // Append and update data
        $('#addItemsTable tbody').append(newRow);

        var items = { 
            MsItemsId: currentNo, 
            MsItemsNama: nama, 
            MsItemsHarga: parseInt(harga), 
            MsItemsTransactionId: transactionId 
        };
        arrayItems.push(items);
        currentNo++;

        // Clear form fields
        $('#Nama').val('');
        $('#Harga').val('');
    });

    $('#itemForm').submit(function (e) {
        e.preventDefault();
        
        $('#descriptionError').text('');
        var description = $('input[name="Description"]').val();

        if (description === '') {
            $('#descriptionError').text('Description is required.');
            return 'Description not filled in';
        }

        var data = {
            Description: description,
            Items: arrayItems
        }
        
        $('#itemsJson').val(JSON.stringify(data));
    });
});