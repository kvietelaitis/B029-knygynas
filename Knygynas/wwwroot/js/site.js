// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
	$(document).on('show.bs.modal', '#workerModal', function (e) {
		var url = $(e.relatedTarget).data('url');
		if (!url) {
			return;
		}

		$('#workerModalContent').load(url, function () {
			var form = $('#workerModalContent').find('form');
			if ($.validator && $.validator.unobtrusive) {
				$.validator.unobtrusive.parse(form);
			}
		});
	});

	$(document).on('submit', '#workerModalContent form', function (e) {
		e.preventDefault();

		var form = $(this);
		$.ajax({
			url: form.attr('action'),
			method: form.attr('method') || 'post',
			data: form.serialize()
		}).done(function (result) {
			if (result && result.redirectUrl) {
				window.location = result.redirectUrl;
				return;
			}

			var modalContent = $('#workerModalContent');
			modalContent.html(result);
			var formAfter = modalContent.find('form');
			if ($.validator && $.validator.unobtrusive) {
				$.validator.unobtrusive.parse(formAfter);
			}
		});
	});

	$(document).on('show.bs.modal', '#deleteModal', function (e) {
		var button = $(e.relatedTarget);
		var url = button.data('delete-url');
		var id = button.data('delete-id');
		var title = button.data('delete-title') || 'Confirm deletion';
		var message = button.data('delete-message') || 'Are you sure you want to delete this item?';

		var modal = $(this);
		modal.find('#deleteModalForm').attr('action', url);
		modal.find('#deleteModalId').val(id);
		modal.find('#deleteModalLabel').text(title);
		modal.find('#deleteModalMessage').text(message);
	});

	$(document).on('click', '.add-to-cart', function () {
		var isbn = $(this).data('isbn');
		if (!isbn) {
			return;
		}

		$.post('/Cart/AddToCart', { isbn: isbn }, function (data) {
			if (data && data.success) {
				$('#cartModalMessage').text('"' + data.title + '" has been added to your cart.');
				$('#cartModal').modal('show');
			}
		});
	});
});
