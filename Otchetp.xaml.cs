﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Регистрация
{
    public partial class Otchetp : Window
    {
        private DataBase DataBase = new DataBase();

        public Otchetp()
        {
            InitializeComponent();
            predm.SelectionChanged += Predm_SelectionChanged;
                
        }

        private void Predm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (predm.SelectedItem == angl)
            {
                fioo.SelectedItem = rad;
            }
            else if (predm.SelectedItem == bd)
            {
                fioo.SelectedItem = anik;
            }
            else if (predm.SelectedItem == fizra)
            {
                fioo.SelectedItem = yal;
            }
            else if (predm.SelectedItem == prog)
            {
                fioo.SelectedItem = dor;

            }
        }

        private void Knopka_Click(object sender, RoutedEventArgs e)
        {
            string predmet = (predm.SelectedItem as ComboBoxItem)?.Content.ToString();
            string fio = (fioo.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(predmet) || string.IsNullOrEmpty(fio))
            {
                MessageBox.Show("Пожалуйста, выберите предмет и ФИО преподавателя.");
                return;
            }

            DataTable table = new DataTable();
            string query = "SELECT name_predmet, fio_prepod, data_pari FROM oneisip22 WHERE name_predmet = @name_predmet AND fio_prepod = @fio_prepod";
            SqlCommand command = new SqlCommand(query, DataBase.ConnectToDatabase());
            command.Parameters.AddWithValue("@name_predmet", predmet);
            command.Parameters.AddWithValue("@fio_prepod", fio);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(table);

            dataGrid.CanUserResizeColumns = false;
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            dataGrid.ItemsSource = table.DefaultView;

            string countQuery = "SELECT COUNT(*) FROM oneisip22 WHERE name_predmet = @name_predmet AND fio_prepod = @fio_prepod";
            SqlCommand countCommand = new SqlCommand(countQuery, DataBase.ConnectToDatabase());
            countCommand.Parameters.AddWithValue("@name_predmet", predmet);
            countCommand.Parameters.AddWithValue("@fio_prepod", fio);

            int count = (int)countCommand.ExecuteScalar();
            vivod_kolvo.Content = $"Количество занятий: {count}";
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            string predmet = (predm.SelectedItem as ComboBoxItem)?.Content.ToString();
            string fio = (fioo.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(predmet) || string.IsNullOrEmpty(fio))
            {
                MessageBox.Show("Пожалуйста, выберите предмет и ФИО преподавателя для создания отчета.");
                return;
            }

            DataTable table = new DataTable();
            string query = "SELECT name_predmet, fio_prepod, data_pari FROM oneisip22 WHERE name_predmet = @name_predmet AND fio_prepod = @fio_prepod";
            SqlCommand command = new SqlCommand(query, DataBase.ConnectToDatabase());
            command.Parameters.AddWithValue("@name_predmet", predmet);
            command.Parameters.AddWithValue("@fio_prepod", fio);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(table);

            int count = table.Rows.Count; // Количество записей в таблице

            // Генерация PDF отчета
            GeneratePDFReport(table, predmet, fio, count);
        }

        private void GeneratePDFReport(DataTable data, string predmet, string fio, int count)
        {
            string fileName = $"Report_{predmet}_{fio}.pdf";
            PdfWriter writer = new PdfWriter(fileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            // Устанавливаем шрифт
            PdfFont font = PdfFontFactory.CreateFont("D:/Загрузки/ARIALUNI.TTF", PdfEncodings.IDENTITY_H);

            // Добавляем заголовок
            Paragraph title = new Paragraph("Отчет о занятиях")
                                    .SetFont(font)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment .CENTER);
            document.Add(title);

            // Добавляем сводную информацию
            Paragraph summary = new Paragraph($"Преподаватель: {fio}, Предмет: {predmet}")
                                    .SetFont(font);
            document.Add(summary);

            // Добавляем количество занятий
            Paragraph countInfo = new Paragraph($"Количество занятий: {count}")
                                    .SetFont(font);
            document.Add(countInfo);

            // Добавляем таблицу
            iText.Layout.Element.Table pdfTable = new iText.Layout.Element.Table(data.Columns.Count);
            // Добавляем заголовки
            foreach (DataColumn column in data.Columns)
            {
                pdfTable.AddHeaderCell(new Cell().Add(new Paragraph(column.ColumnName).SetFont(font)));
            }
            // Добавляем данные
            foreach (DataRow row in data.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    pdfTable.AddCell(new Cell().Add(new Paragraph(item.ToString()).SetFont(font)));
                }
            }
            document.Add(pdfTable);

            document.Close();

            MessageBox.Show($"Отчет сохранен в файл: {fileName}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


    }
}
