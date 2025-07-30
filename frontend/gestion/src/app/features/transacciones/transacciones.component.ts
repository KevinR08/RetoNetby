
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Transaccion } from '../../core/models/transaccion.model';
import { Producto } from '../../core/models/producto.model';
import { TransaccionService } from '../../core/services/transaccion.service';
import { ProductoService } from '../../core/services/producto.service';


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
  detalleVisible = false;
  transaccionSeleccionada: Transaccion | null = null;

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

  ver(t: Transaccion) {
    this.transaccionSeleccionada = t;
    this.detalleVisible = true;
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
    this.modo = 'nuevo';
    this.form = this.vacio();
    this.dialogoVisible = true;
  }

  editar(t: Transaccion) {
    this.modo = 'editar';
    this.form = { ...t };
    this.dialogoVisible = true;
  }

  guardar() {
    if (this.form.tipo === 'Venta') {
      const producto = this.productos.find(p => p.id?.toString() === this.form.productoId?.toString());

      if (producto && this.form.cantidad > producto.stock) {
        this.toast.add({
          severity: 'warn',
          summary: 'Stock insuficiente',
          detail: `Solo hay ${producto.stock} unidades disponibles.`
        });
        return;
      }
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
        const backendMessage = err?.error;
        this.toast.add({
          severity: 'error',
          summary: 'Error',
          detail: backendMessage || 'No se pudo guardar la transacción.'
        });
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

  cerrarYCargar() {
    this.cerrar();
    this.buscar();
  }

  obtenerNombreProducto(id: number | string | undefined): string {
    const producto = this.productos.find(p => p.id?.toString() === id?.toString());
    return producto?.nombre || 'N/A';
  }
}
