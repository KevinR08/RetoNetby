// src/app/features/transacciones/transacciones.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Transaccion } from '../../core/models/transaccion.model';
import { Producto } from '../../core/models/producto.model';
import { TransaccionService } from '../../core/services/transaccion.service';
import { ProductoService } from '../../core/services/producto.service';

/* PrimeNG */
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { MessageService } from 'primeng/api';



@Component({
  standalone: true,
  selector: 'app-transacciones',
  templateUrl: './transacciones.component.html',
  styleUrls: ['./transacciones.component.scss'],
  providers: [MessageService],
  imports: [
    CommonModule, FormsModule,
    DialogModule, TableModule, ButtonModule, ToastModule,
    DropdownModule, InputNumberModule, CalendarModule
  ]
})
export class TransaccionesComponent {
  transacciones: Transaccion[] = [];
  productos: Producto[] = [];
  dialogoVisible = false;
modo: 'nuevo' | 'editar' = 'nuevo';

  filtro = {
    productoId: 0,
    tipo: '',
    inicio: '',
    fin: ''
  };

  form: Transaccion = this.vacio();

  constructor(
    private transacSrv: TransaccionService,
    private productoSrv: ProductoService,
    private toast: MessageService
  ) {
    this.productoSrv.getAll().subscribe({ next: p => this.productos = p });
    this.buscar();
  }

  private vacio(): Transaccion {
    return {
      fecha: new Date().toISOString(),
      tipo: 'Compra',
      productoId: 0,
      cantidad: 0,
      precioUnitario: 0
    };
  }

  buscar() {
  const { productoId, tipo, inicio, fin } = this.filtro;
  this.transacSrv.getAll({
    productoId: productoId || undefined,
    tipo: tipo || undefined,
    inicio: inicio ? new Date(inicio).toISOString() : undefined,
    fin: fin ? new Date(fin).toISOString() : undefined,
  }).subscribe({
    next: t => this.transacciones = t,
    error: e => {
      console.error(e);
      this.toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'No se pudieron cargar las transacciones.'
      });
    }
  });
}


  abrir() {
    this.modo='nuevo';
    this.form = this.vacio();
    this.dialogoVisible = true;
  }

   editar(t: Transaccion) {
    this.modo = 'editar';
    this.form = { ...t };
    this.dialogoVisible = true;
  }

  guardar() {
    if (!this.form.productoId || !this.form.tipo || this.form.cantidad <= 0 || this.form.precioUnitario <= 0) {
      this.toast.add({ severity: 'warn', summary: 'Validación', detail: 'Completa todos los campos obligatorios.' });
      return;
    }

    const op = this.modo === 'editar' && this.form.id
      ? this.transacSrv.update(this.form)
      : this.transacSrv.create(this.form);

    op.subscribe({
      next: () => {
        const mensaje = this.modo === 'editar' ? 'actualizada' : 'registrada';
        this.toast.add({ severity: 'success', summary: 'Éxito', detail: `Transacción ${mensaje} correctamente.` });
        this.cerrarYCargar();
      },
      error: err => {
        console.error(err);
        this.toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar la transacción.' });
      }
    });
  }

  eliminar(id?: number) {
    if (!id) return;
    this.transacSrv.delete(id).subscribe({
      next: () => {
        this.toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Transacción eliminada.' });
        this.buscar();
      },
      error: err => {
        console.error(err);
        this.toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.' });
      }
    });
  }

  cerrar() {
    this.dialogoVisible = false;
  }

  private cerrarYCargar() {
    this.cerrar();
    this.buscar();
  }
}
