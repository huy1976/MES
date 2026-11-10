import React, { useState, useCallback, useEffect, useRef } from "react";
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell, Legend,
  LineChart, Line
} from 'recharts';
import "./AIChatBox.css";
import * as XLSX from 'xlsx';
import html2canvas from 'html2canvas';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import { jsPDF } from "jspdf";

// --- CÁC HÀM TIỆN ÍCH (UTILITIES) BẢO VỆ HỆ THỐNG ---

const safeJsonParse = (str) => {
  try {
    let cleanStr = str.replace(/""/g, '"').replace(/,\s*}/g, '}').replace(/,\s*\]/g, ']');
    return JSON.parse(cleanStr);
  } catch (e) {
    return null;
  }
};

const parseAIResponse = (rawText) => {
  let result = {
    displayText: rawText,
    isChart: false,
    chartType: "Bar",
    chartData: [],
    isExport: false,
    exportQuery: ""
  };

  if (!rawText) return result;

  try {
    const detectChartType = (text) => {
      const s = text.toLowerCase();
      if (s.includes("pie") || s.includes("tròn")) return "Pie";
      if (s.includes("line") || s.includes("đường")) return "Line";
      return "Bar";
    };

    const extractDataSafely = (text) => {
      const extracted = [];
      const lines = text.split('\n');
      let currentGroup = "";

      lines.forEach(line => {
        if (!line.trim().startsWith('-') && line.includes(':')) {
          // Lấy vị trí dấu ':' cuối cùng (Đề phòng dấu ':' nằm trong chữ (ID: 1))
          let fullGroupName = line.substring(0, line.lastIndexOf(':')); 
          let groupName = fullGroupName
            .replace(/^\d+\.\s*/, '')     // Xóa số thứ tự (VD: "1. ")
            .replace(/\*\*/g, '')         // Xóa dấu in đậm
            .replace(/\s*\([^)]*(\)|$)/g, '') // Xóa triệt để cụm (ID: xxx)
            .trim();
          if (groupName) currentGroup = groupName;
        }
        const match = line.match(/-\s*(.*):\s*([\d,\.]+)/);
        if (match) {
          let metricName = match[1].replace(/\s*\([^)]*\)/g, '').replace(/[\*\_]/g, '').trim();
          let rawNumber = match[2].replace(/[,.]/g, ''); // Xóa dấu phân cách ngàn
          let parsedValue = parseInt(rawNumber, 10);
          
          if (metricName && !isNaN(parsedValue)) {
            // Ghép tên thiết bị với loại dữ liệu. VD: "Cảm biến tiệm cận (Số lượng)"
            let finalName = currentGroup ? `${currentGroup} (${metricName})` : metricName;
            extracted.push({ name: finalName, value: parsedValue });
          }
        }
      });
      return extracted;
    };

    const chartRegex1 = /\[CHART_READY\|(.*?)\]\s*(\[[\s\S]*\])/;
    const actionRegex = /(\{\s*"{1,2}Action"{1,2}\s*:[\s\S]*?\})/;

    const match1 = rawText.match(chartRegex1);
    const match2 = rawText.match(actionRegex);

    if (match1) {
      result.isChart = true;
      result.chartType = detectChartType(match1[1]);
      const rawData = safeJsonParse(match1[2]);
if (Array.isArray(rawData)) {
        result.chartData = rawData.map(item => {
          const ObjectKeys = Object.keys(item);
          let val = String(item[ObjectKeys[1]]).replace(/[,.]/g, '');
          return { 
            name: String(item[ObjectKeys[0]] || "Unknown").trim(), 
            value: parseInt(val, 10) || 0 
          };
        }).filter(d => !isNaN(d.value));
      }
      result.displayText = rawText.replace(chartRegex1, '').trim();

    } else if (match2) {
      const jsonConfig = safeJsonParse(match2[1]);
      result.displayText = rawText.replace(actionRegex, '').trim();

      if (jsonConfig) {
        if (jsonConfig.Action === "Chart" || result.displayText.toLowerCase().includes("biểu đồ")) {
          result.isChart = true;
          result.chartType = detectChartType(jsonConfig.ChartType || result.displayText);
          result.chartData = extractDataSafely(rawText);
        }

        if (jsonConfig.Action === "NewQueryAndExport") {
          result.isExport = true;
          result.exportQuery = jsonConfig.Query || "No query provided by AI.";
        }
      }
    } else {
      if (result.displayText.toLowerCase().includes("biểu đồ")) {
        result.isChart = true;
        result.chartType = detectChartType(result.displayText);
        result.chartData = extractDataSafely(rawText);
      }
    }
  } catch (error) {
    console.warn("Parse Engine Error (Ignored):", error);
    result.displayText = rawText; 
  }

  if (result.isChart && result.chartData.length === 0) {
    result.isChart = false; 
  }

  return result;
};

// ============================================================================
// COMPONENT CHÍNH
// ============================================================================

const AIChatBox = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    {
      id: Date.now(),
      type: "bot",
      text: "Xin chào! Tôi là trợ lý AI. Bạn có thể gửi tin nhắn, hình ảnh hoặc tài liệu cho tôi nhé!",
    },
  ]);
  const [input, setInput] = useState("");
  const [isTyping, setIsTyping] = useState(false);
  const [enlargedChart, setEnlargedChart] = useState(null);
  
  const PIE_COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#AF19FF', '#FF19A3'];
  
  const messagesEndRef = useRef(null);
  const inputRef = useRef(null);
  const chatWrapperRef = useRef(null);
  const fileInputRef = useRef(null);

  const toggleChat = useCallback(() => setIsOpen((prev) => !prev), []);

  useEffect(() => {
    if (isOpen && !isTyping) {
      setTimeout(() => inputRef.current?.focus(), 400);
    }
  }, [isOpen, isTyping]);

  useEffect(() => {
    if (inputRef.current) {
      inputRef.current.style.height = "auto";
      const newHeight = Math.min(inputRef.current.scrollHeight, 150);
      inputRef.current.style.height = `${newHeight}px`;
    }
  }, [input]);
const scrollToBottom = () => messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });

  useEffect(() => {
    scrollToBottom();
  }, [messages, isTyping]);

  useEffect(() => {
    const handleMove = (e) => {
      if (!chatWrapperRef.current) return;
      const pupils = chatWrapperRef.current.querySelectorAll(".eye-pupil");
      pupils.forEach((eye) => {
        const rect = eye.getBoundingClientRect();
        const x = rect.left + rect.width / 2;
        const y = rect.top + rect.height / 2;
        const rad = Math.atan2(e.clientY - y, e.clientX - x);
        const moveX = Math.cos(rad) * 3; 
        const moveY = Math.sin(rad) * 3;
        eye.style.transform = `translate(${moveX}px, ${moveY}px)`;
      });
    };
    window.addEventListener("mousemove", handleMove);
    return () => window.removeEventListener("mousemove", handleMove);
  }, []);

  const formatText = (text) => {
    if (!text) return null;
    const parts = text.split(/(\*\*.*?\*\*)/g);
    return parts.map((part, index) => {
      if (part.startsWith('**') && part.endsWith('**')) {
        return <strong key={`bold-${index}`}>{part.slice(2, -2)}</strong>;
      }
      return <span key={`text-${index}`}>{part}</span>;
    });
  };
  const handleExportImage = async (chartId, fileName = "Bieu_Do_AI.png") => {
    if (!chartId) return;
    const chartElement = document.getElementById(chartId);
    if (!chartElement) return;

    try {
      // Chụp riêng cái thẻ chứa biểu đồ
      const canvas = await html2canvas(chartElement, { scale: 2, backgroundColor: "#ffffff" });
      const imgData = canvas.toDataURL("image/png");
      
      // Tạo link tải xuống tự động
      const link = document.createElement("a");
      link.href = imgData;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (err) {
      console.error("Lỗi xuất ảnh:", err);
      alert("Đã xảy ra lỗi khi tải ảnh biểu đồ!");
    }
  };

  const handleExportExcel = async (chartData, chartId, fileName = "Bao_Cao_AI.xlsx") => {
    if (!chartData || chartData.length === 0) return alert("Không có dữ liệu!");
    
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Báo Cáo Data');

    worksheet.columns = [
      { header: 'Tên Thiết Bị / Linh Kiện', key: 'name', width: 35 },
      { header: 'Số Lượng', key: 'value', width: 15 }
    ];
    worksheet.addRows(chartData);
    worksheet.getRow(1).font = { bold: true }; 

    if (chartId) {
      const chartElement = document.getElementById(chartId);
      if (chartElement) {
        // Tăng scale lên 3 để nét căng
        const canvas = await html2canvas(chartElement, { scale: 3, backgroundColor: "#ffffff" });
        const base64Image = canvas.toDataURL("image/png");
        const imageId = workbook.addImage({ base64: base64Image, extension: 'png' });
// Tính toán tỷ lệ khung hình chuẩn để ảnh không bị bóp méo
        const targetWidth = 600; 
        const targetHeight = Math.round((canvas.height / canvas.width) * targetWidth);

        worksheet.addImage(imageId, { 
          tl: { col: 3, row: 1 }, 
          ext: { width: targetWidth, height: targetHeight } 
        });
      }
    }

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName);
  };

  const handleExportWord = async (text, chartId, fileName = "Bao_Cao_AI.doc") => {
    let imageHtml = "";
    if (chartId) {
      const chartElement = document.getElementById(chartId);
      if (chartElement) {
        const canvas = await html2canvas(chartElement, { scale: 2, backgroundColor: "#ffffff" });
        const imgData = canvas.toDataURL("image/png");
        
        // Tính toán tỷ lệ. MS Word rất dốt CSS, bắt buộc dùng thẻ width, height thuần
        const imgWidth = 500;
        const imgHeight = Math.round((canvas.height / canvas.width) * imgWidth);

        imageHtml = `<br><br><div align="center"><img src="${imgData}" width="${imgWidth}" height="${imgHeight}" /></div>`;
      }
    }

    const content = `
      <html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>
      <head><meta charset='utf-8'><title>Báo Cáo AI</title></head>
      <body>
        <div style="font-family: Arial, sans-serif; font-size: 13pt; line-height: 1.5;">
          <h2 style="color: #1565c0; text-align: center;">BÁO CÁO TỪ TRỢ LÝ AI</h2>
          <div>${text.replace(/\n/g, '<br>')}</div>
          ${imageHtml}
        </div>
      </body>
      </html>
    `;
    const blob = new Blob(['\ufeff', content], { type: 'application/msword' });
    saveAs(blob, fileName);
  };

  const handleExportPDF = async (messageId, fileName = "Bao_Cao_AI.pdf") => {
    const element = document.getElementById(`msg-export-area-${messageId}`);
    if (!element) return;

    const buttons = document.getElementById(`export-buttons-${messageId}`);
    if (buttons) buttons.style.display = 'none';

    try {
      const canvas = await html2canvas(element, { scale: 2, backgroundColor: "#ffffff" });
      const imgData = canvas.toDataURL('image/png');
      
      const pdf = new jsPDF('p', 'mm', 'a4');
      let pdfWidth = pdf.internal.pageSize.getWidth() - 20; 
      let pdfHeight = (canvas.height * pdfWidth) / canvas.width;
      
      // Chống tràn trang PDF: Nếu quá dài thì thu nhỏ lại cho vừa 1 trang
      const maxPdfHeight = pdf.internal.pageSize.getHeight() - 30;
      if(pdfHeight > maxPdfHeight) {
          pdfHeight = maxPdfHeight;
          pdfWidth = (canvas.width * pdfHeight) / canvas.height;
      }

      pdf.setFontSize(16);
      pdf.setTextColor(21, 101, 192); 
      pdf.text("BAO CAO PHAN TICH TU AI", 105, 15, { align: "center" });
      
      // Canh giữa ảnh nếu bị thu nhỏ
const marginX = (pdf.internal.pageSize.getWidth() - pdfWidth) / 2;
      pdf.addImage(imgData, 'PNG', marginX, 25, pdfWidth, pdfHeight);
      pdf.save(fileName);
    } finally {
      if (buttons) buttons.style.display = 'flex';
    }
  };

  const handleSend = async () => {
    const userQuestion = input.trim();
    if (!userQuestion || isTyping) return;

    const userMessage = { id: Date.now(), type: "user", text: userQuestion };
    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    if (inputRef.current) inputRef.current.style.height = "auto";
    setIsTyping(true);

    try {
      // Lấy JWT đã được lưu sau khi người dùng đăng nhập.
      // Kiểm tra một số tên thường dùng trong cả localStorage và sessionStorage.
      const accessToken =
        localStorage.getItem("accessToken") ||
        localStorage.getItem("token") ||
        localStorage.getItem("jwtToken") ||
        sessionStorage.getItem("accessToken") ||
        sessionStorage.getItem("token") ||
        sessionStorage.getItem("jwtToken");

      if (!accessToken) {
        throw new Error("Không tìm thấy JWT đăng nhập. Hãy đăng nhập lại.");
      }

      const response = await fetch('https://localhost:7260/query', {
        method: 'POST',
        headers: {
          'accept': 'text/plain',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify({
          question: userQuestion,
          topK: 100
        })
      });

      if (!response.ok) throw new Error(`Lỗi Server: ${response.status}`);

      const rawText = await response.text();
      const parsedData = parseAIResponse(rawText);

      setMessages((prev) => [
        ...prev,
        { 
          id: Date.now(), 
          type: "bot", 
          text: parsedData.displayText || "Dưới đây là kết quả của bạn:", 
          isChart: parsedData.isChart,
          chartType: parsedData.chartType,
          chartData: parsedData.chartData,
          isExport: parsedData.isExport,
          exportQuery: parsedData.exportQuery
        }
      ]);
    } catch (error) {
      console.error("API Error:", error);
      setMessages((prev) => [
        ...prev,
        { id: Date.now(), type: "bot", text: "Xin lỗi, đã có lỗi kết nối hoặc server không phản hồi. 🛠️" }
      ]);
    } finally {
      setIsTyping(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (!file || isTyping) return;

    const fileUrl = URL.createObjectURL(file);
    const isImage = file.type.startsWith("image/");
    const messageId = Date.now();

    setMessages((prev) => [...prev, {
      id: messageId, type: "user", text: `Đang tải lên: ${file.name}`,
      fileUrl: fileUrl, fileType: isImage ? "image" : "file", fileName: file.name, progress: 0 
    }]);
    setIsTyping(true);

    const formData = new FormData();
    formData.append("files", file);
    const xhr = new XMLHttpRequest();
    xhr.open("POST", "https://localhost:7260/ingest/upload", true);

    xhr.upload.onprogress = (event) => {
      if (event.lengthComputable) {
        const percent = Math.round((event.loaded * 100) / event.total);
        setMessages((prev) => prev.map((msg) => msg.id === messageId ? { ...msg, progress: percent } : msg));
      }
    };

    xhr.onload = () => {
      setIsTyping(false);
      if (xhr.status >= 200 && xhr.status < 300) {
setMessages((prev) => [...prev, { id: Date.now(), type: "bot", text: `✅ Đã tiếp nhận tệp: ${file.name}` }]);
      } else {
        setMessages((prev) => [...prev, { id: Date.now(), type: "bot", text: `❌ Lỗi tải tệp ${file.name}` }]);
      }
    };

    xhr.onerror = () => {
      setIsTyping(false);
      setMessages((prev) => [...prev, { id: Date.now(), type: "bot", text: `❌ Lỗi mạng khi tải tệp.` }]);
    };

    xhr.send(formData);
    e.target.value = null; 
  };

  return (
    <div className={`ai-chat-wrapper ${isOpen ? "open" : ""}`} ref={chatWrapperRef}>
      <div className="chat-window">
        <div className="chat-header">
          <div className="header-info">
            <span className="online-dot"></span>
            <span>Trợ lý AI</span>
          </div>
          <button onClick={toggleChat} className="close-btn" title="Đóng chat">×</button>
        </div>

        <div className="chat-content">
          {messages.map((msg) => (
            <div key={msg.id} className={`msg ${msg.type === "bot" ? "bot-msg" : "user-msg"}`}>
              {msg.fileType === "image" && <img src={msg.fileUrl} alt="upload" className="chat-image-preview" />}
              {msg.fileType === "file" && (
                <div className="chat-file-attachment">
                  <span>
                    {msg.fileName?.toLowerCase().match(/\.(xlsx?|csv)$/) ? "📊" : 
                     msg.fileName?.toLowerCase().endsWith(".pdf") ? "📕" : "📁"}
                  </span>
                  <a href={msg.fileUrl} download={msg.fileName}>{msg.fileName}</a>
                </div>
              )}
              
              <div className="text-wrapper">
                <div id={`msg-export-area-${msg.id}`}>
                  <div style={{ whiteSpace: "pre-wrap" }}>{formatText(msg.text)}</div>

                  {msg.isExport && msg.exportQuery && (
                    <div className="chat-export-box" style={{ marginTop: '15px', padding: '12px', backgroundColor: '#e3f2fd', borderRadius: '8px', border: '1px solid #90caf9' }}>
                      <div style={{ fontWeight: 'bold', marginBottom: '8px', fontSize: '13px', color: '#1565c0' }}>📄 Câu lệnh xuất dữ liệu</div>
                      <code style={{ fontSize: '11px', color: '#c62828', wordBreak: 'break-all', display: 'block', backgroundColor: 'white', padding: '5px', borderRadius: '4px' }}>
                        {msg.exportQuery}
                      </code>
                    </div>
                  )}

                  {msg.isChart && msg.chartData?.length > 0 && (
                    <div 
                      id={`chart-container-${msg.id}`} 
                      className="chat-chart-container" 
                      style={{ width: '100%', height: 260, marginTop: '15px', cursor: 'pointer', backgroundColor: 'white' }}
                      onClick={() => setEnlargedChart({ chartType: msg.chartType, chartData: msg.chartData })}
title="Click để phóng to biểu đồ"
                    >
                      <ResponsiveContainer width="100%" height="100%">
                        {msg.chartType === 'Bar' && (
                          <BarChart data={msg.chartData} margin={{ top: 10, right: 10, left: -20, bottom: 25 }}>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} />
                            <XAxis dataKey="name" interval={0} tick={{fontSize: 11, angle: -30, textAnchor: 'end'}} />
                            <YAxis tick={{fontSize: 12}} />
                            <Tooltip cursor={{fill: 'transparent'}} />
                            <Bar dataKey="value" fill="#4caf50" radius={[4, 4, 0, 0]} barSize={35} />
                          </BarChart>
                        )}
                        {msg.chartType === 'Pie' && (
                          <PieChart>
                            <Pie data={msg.chartData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={80} label>
                              {msg.chartData.map((entry, index) => <Cell key={`cell-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />)}
                            </Pie>
                            <Tooltip />
                            <Legend wrapperStyle={{ fontSize: '11px', paddingTop: '10px' }} />
                          </PieChart>
                        )}
                        {msg.chartType === 'Line' && (
                          <LineChart data={msg.chartData} margin={{ top: 10, right: 10, left: -20, bottom: 25 }}>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} />
                            <XAxis dataKey="name" interval={0} tick={{fontSize: 11, angle: -30, textAnchor: 'end'}} />
                            <YAxis tick={{fontSize: 12}} />
                            <Tooltip />
                            <Line type="monotone" dataKey="value" stroke="#2196f3" strokeWidth={3} dot={{ r: 5 }} activeDot={{ r: 8 }} />
                          </LineChart>
                        )}
                      </ResponsiveContainer>
                    </div>
                  )}

                  {msg.isChart && (!msg.chartData || msg.chartData.length === 0) && (
                    <div style={{color: '#ff9800', fontSize: '12px', marginTop: '10px', fontStyle: 'italic'}}>
                      *Hệ thống đã nhận lệnh vẽ biểu đồ {msg.chartType} nhưng chưa thể trích xuất số liệu.
                    </div>
                  )}
                </div> 

                {msg.isChart && msg.chartData?.length > 0 && (
                  <div id={`export-buttons-${msg.id}`} style={{ display: 'flex', gap: '8px', marginTop: '12px' }}>
                    <button 
                      onClick={() => handleExportExcel(msg.chartData, `chart-container-${msg.id}`, `BaoCao_${msg.id}.xlsx`)}
style={{ flex: 1, padding: '6px', fontSize: '11px', cursor: 'pointer', backgroundColor: '#217346', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '5px' }}>
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
                      Excel
                    </button>
                    <button 
                      onClick={() => handleExportWord(msg.text, `chart-container-${msg.id}`, `BaoCao_${msg.id}.doc`)}
                      style={{ flex: 1, padding: '6px', fontSize: '11px', cursor: 'pointer', backgroundColor: '#2b579a', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '5px' }}>
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
                      Word
                    </button>
                    <button 
                      onClick={() => handleExportPDF(msg.id, `BaoCao_${msg.id}.pdf`)}
                      style={{ flex: 1, padding: '6px', fontSize: '11px', cursor: 'pointer', backgroundColor: '#d32f2f', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '5px' }}>
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
                      PDF
                    </button>
                    <button 
                      onClick={() => handleExportImage(`chart-container-${msg.id}`, `BieuDo_${msg.id}.png`)}
                      style={{ flex: 1, padding: '6px', fontSize: '11px', cursor: 'pointer', backgroundColor: '#f57c00', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '5px' }}>
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                      Ảnh
                    </button>
</div>
                )}
              </div>
            </div>
          ))}

          {isTyping && (
            <div className="bot-msg msg typing">
              <span></span><span></span><span></span>
            </div>
          )}
          <div ref={messagesEndRef}></div>
        </div>

        <div className="chat-footer">
          <input 
            type="file" 
            ref={fileInputRef} 
            style={{ display: "none" }} 
            onChange={handleFileChange}
            accept="image/*, .pdf, .doc, .docx, .txt, .xls, .xlsx"
            disabled={isTyping}
          />
          <button 
            className="attach-btn" 
            onClick={() => fileInputRef.current.click()} 
            title="Gửi tệp"
            disabled={isTyping}
            style={{ opacity: isTyping ? 0.5 : 1 }}
          >
            +
          </button>
          
          <textarea
            ref={inputRef}
            rows="1"
            value={input}
            placeholder={isTyping ? "AI đang trả lời..." : "Nhập câu hỏi..."}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
            className="chat-textarea"
            disabled={isTyping}
          />

          <button 
            className="send-btn" 
            onClick={handleSend}
            disabled={isTyping || !input.trim()}
            style={{ opacity: (isTyping || !input.trim()) ? 0.5 : 1 }}
          >
            Gửi
          </button>
        </div>
      </div>

      <div className="chat-trigger" onClick={toggleChat}>
        <svg width="35" height="35" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="bot-svg">
          <path d="M12 8V4H8" />
          <rect width="16" height="12" x="4" y="8" rx="2" />
          <path d="M2 14h2M20 14h2" />
          <g className="eye-socket">
            <circle cx="8.5" cy="13.5" r="1.5" stroke="rgba(255,255,255,0.3)" />
            <circle cx="8.5" cy="13.5" r="0.8" fill="white" className="eye-pupil" />
          </g>
          <g className="eye-socket">
            <circle cx="15.5" cy="13.5" r="1.5" stroke="rgba(255,255,255,0.3)" />
            <circle cx="15.5" cy="13.5" r="0.8" fill="white" className="eye-pupil" />
          </g>
        </svg>
      </div>

      {enlargedChart && (
        <div className="chart-modal-overlay" onClick={() => setEnlargedChart(null)}>
          <div className="chart-modal-content" onClick={(e) => e.stopPropagation()}>
            <button className="chart-modal-close" onClick={() => setEnlargedChart(null)}>×</button>
            <h3 style={{ marginTop: 0, textAlign: 'center', color: '#1565c0' }}>Chi tiết biểu đồ</h3>
            
            <div style={{ width: '100%', height: '400px' }}>
              <ResponsiveContainer width="100%" height="100%">
                {enlargedChart.chartType === 'Bar' && (
<BarChart data={enlargedChart.chartData} margin={{ top: 20, right: 20, left: 0, bottom: 30 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} />
                    <XAxis dataKey="name" interval={0} tick={{ fontSize: 13, angle: -25, textAnchor: 'end' }}/>
                    <YAxis />
                    <Tooltip cursor={{fill: 'transparent'}} />
                    <Bar dataKey="value" fill="#4caf50" radius={[4, 4, 0, 0]} barSize={50} label={{ position: 'top', fill: '#666' }} />
                  </BarChart>
                )}
                {enlargedChart.chartType === 'Pie' && (
                  <PieChart>
                    <Pie data={enlargedChart.chartData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={130} label>
                      {enlargedChart.chartData.map((entry, index) => <Cell key={`cell-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />)}
                    </Pie>
                    <Tooltip />
                    <Legend wrapperStyle={{ paddingTop: '20px' }} />
                  </PieChart>
                )}
                {enlargedChart.chartType === 'Line' && (
                  <LineChart data={enlargedChart.chartData} margin={{ top: 20, right: 20, left: 0, bottom: 30 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} />
                    <XAxis dataKey="name" interval={0} tick={{ fontSize: 13, angle: -25, textAnchor: 'end' }}/>
                    <YAxis />
                    <Tooltip />
                    <Line type="monotone" dataKey="value" stroke="#2196f3" strokeWidth={4} dot={{ r: 6 }} activeDot={{ r: 10 }} />
                  </LineChart>
                )}
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AIChatBox;